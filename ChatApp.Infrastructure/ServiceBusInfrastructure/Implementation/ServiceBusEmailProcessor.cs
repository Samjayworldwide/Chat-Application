using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ChatApp.Infrastructure.configurations;
using ChatApp.Infrastructure.EmailInfrastructure.Interface;
using ChatApp.Infrastructure.models;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.ServiceBusInfrastructure.Implementation;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class ServiceBusEmailProcessor : IServiceBusProcessorBase
{
    private readonly ServiceBusSettings _serviceBusSettings;

    private readonly IEmailService _emailService;

    private readonly ServiceBusClient _serviceBusClient;

    private ServiceBusProcessor? _serviceProcessor;

    private readonly DateTime _dateTime = DateTime.UtcNow.AddHours(1);

    private readonly ILogger<ServiceBusEmailProcessor> _logger;

    public ServiceBusEmailProcessor(IOptions<ServiceBusSettings> options,
        IEmailService emailService, ServiceBusClient serviceBusClient, ILogger<ServiceBusEmailProcessor> logger)
    {
        _serviceBusSettings = options.Value ?? throw new ArgumentNullException(nameof(options));

        _emailService = emailService;

        _serviceBusClient = serviceBusClient;

        _logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProcessor != null)
        {
            await _serviceProcessor.StopProcessingAsync();

            await _serviceProcessor.DisposeAsync();
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            };

            _serviceProcessor =
                _serviceBusClient.CreateProcessor(_serviceBusSettings.EmailQueueName, serviceBusProcessorOptions);

            _serviceProcessor.ProcessMessageAsync += ProcessEmail;

            _serviceProcessor.ProcessErrorAsync += ErrorHandler;

            await _serviceProcessor.StartProcessingAsync(cancellationToken);

            _logger.LogInformation("Started processing email");
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Unable to process message from queue");
        }
    }

    private async Task ProcessEmail(ProcessMessageEventArgs processMessageEventArgs)
    {
        try
        {
            var body = processMessageEventArgs.Message.Body.ToString();

            _logger.LogInformation("This is the message {message} received at {Time}", body,
                _dateTime.ToString(CultureInfo.InvariantCulture));

            if (string.IsNullOrEmpty(body))
            {
                _logger.LogInformation("Message body received from queue is null or empty");

                await processMessageEventArgs.DeadLetterMessageAsync(processMessageEventArgs.Message);

                return;
            }

            var emailDetails = JsonSerializer.Deserialize<EmailDetails>(body);

            if (emailDetails == null)
            {
                _logger.LogInformation("Email details received from queue is null");

                await processMessageEventArgs.DeadLetterMessageAsync(processMessageEventArgs.Message);

                return;
            }

            var result = await _emailService.SendEmailAsync(emailDetails);

            if (!result)
            {
                _logger.LogInformation("Email sending failed");

                await processMessageEventArgs.AbandonMessageAsync(processMessageEventArgs.Message);

                return;
            }

            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing email in the queue");

            await processMessageEventArgs.AbandonMessageAsync(processMessageEventArgs.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
    {
        _logger.LogError(processErrorEventArgs.Exception, "Error processing email");

        return Task.CompletedTask;
    }
}