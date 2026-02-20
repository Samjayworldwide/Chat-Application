using ChatApp.WebApi.BackgroundWorkers;
using ChatApp.WebApi.Controllers;
using ChatApp.WebApi.extensions;
using ChatApp.WebApi.Middleware;
using ChatApp.WebApi.utilities;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; }).AddStackExchangeRedis("localhost:6379",
    options => { options.Configuration.ChannelPrefix = RedisChannel.Literal("ChatApp"); });

builder.Services.RegisterStackExchangeRedisCache(builder.Configuration);

builder.Services.RegisterRedisPubSubService();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.RegisterConfigurations(builder.Configuration);

builder.Services.RegisterMongoDb();

builder.Services.RegisterRepositories();

builder.Services.RegisterServices();

builder.Services.AddHostedService<ServiceBusWorker>();

builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();

builder.Services.AddAuthorization();

builder.Services.RegisterAuthentication(builder.Configuration);

builder.Services.RegisterServiceBusInfrastructure(builder.Configuration);

builder.Services.RegisterBlobStorageInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IUserIdProvider, ChatUserIdProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(_ => { });

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat-hub");

app.Run();