using System.Diagnostics.CodeAnalysis;
using System.Text;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using ChatApp.Application.commons;
using ChatApp.Application.implementations;
using ChatApp.Application.interfaces;
using ChatApp.Infrastructure.BlobStorageInfrastructure.Implementation;
using ChatApp.Infrastructure.BlobStorageInfrastructure.Interfaces;
using ChatApp.Infrastructure.configurations;
using ChatApp.Infrastructure.EmailInfrastructure.Implementation;
using ChatApp.Infrastructure.EmailInfrastructure.Interface;
using ChatApp.Infrastructure.RedisInfrastructure.Implementation;
using ChatApp.Infrastructure.RedisInfrastructure.Interfaces;
using ChatApp.Infrastructure.repositories.implementations;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Implementation;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;
using ChatApp.SharedKernel.extensions;
using ChatApp.WebApi.utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;

namespace ChatApp.WebApi.extensions;

[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterConfigurations(IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));

            services.Configure<ServiceBusSettings>(configuration.GetSection(nameof(ServiceBusSettings)));

            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

            services.Configure<BlobStorageSettings>(configuration.GetSection(nameof(BlobStorageSettings)));
        }

        public void RegisterServiceBusInfrastructure(IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(_ =>
                new ServiceBusClient(configuration["ServiceBusSettings:ConnectionString"]));

            services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

            services.AddScoped<IServiceBusProcessorBase, ServiceBusEmailProcessor>();
        }

        public void RegisterBlobStorageInfrastructure(IConfiguration configuration)
        {
            services.AddSingleton<BlobServiceClient>(_ =>
                new BlobServiceClient(configuration["BlobStorageSettings:ConnectionString"]));
        }

        public void RegisterRedisPubSubService()
        {
            services.AddScoped<IRedisPubSubService, RedisPubSubService>();
        }

        public void RegisterMongoDb()
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();

                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

                return client.GetDatabase(settings.DatabaseName);
            });
        }

        public void RegisterRepositories()
        {
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();

            services.AddScoped<IPrivateMessageRepository, PrivateMessageRepository>();

            services.AddScoped<IGroupRepository, GroupRepository>();

            services.AddScoped<IGroupMessageRepository, GroupMessageRepository>();

            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddScoped<IChatNotifier, ChatNotifier>();
        }

        public void RegisterStackExchangeRedisCache(IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var configOptions = ConfigurationOptions.Parse(
                    configuration.GetConnectionString("RedisConnection")!);

                configOptions.AbortOnConnectFail = false;
                configOptions.ConnectRetry = 3;
                configOptions.ConnectTimeout = 5000;
                configOptions.SyncTimeout = 5000;
                configOptions.AsyncTimeout = 5000;

                return ConnectionMultiplexer.Connect(configOptions);
            });
        }

        public void RegisterServices()
        {
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<IGroupService, GroupService>();

            services.AddScoped<IBlobStorageService, BlobStorageService>();

            services.AddScoped<IUserService, UserService>();
        }

        public void RegisterAuthentication(IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                    };

                    var apiResponse = Result<string>.Failure("Token is either expired or invalid");

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat-hub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = async context =>
                        {
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                                context.Response.ContentType = "application/json";

                                await context.Response.WriteAsJsonAsync(apiResponse);
                            }
                        },
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                                context.Response.ContentType = "application/json";

                                await context.Response.WriteAsJsonAsync(apiResponse);
                            }
                        },
                        OnTokenValidated = _ => Task.CompletedTask
                    };
                });
        }
    }
}