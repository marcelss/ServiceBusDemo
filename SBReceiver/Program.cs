using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SBShared.Models;

namespace SBReceiver
{
    class Program
    {
        private const string queueName = "personqueue";
        // the client that owns the connection and can be used to create senders and receivers
        static ServiceBusClient _queueClient;

        // the processor that reads and processes messages from the queue
        static ServiceBusProcessor _processor;

        static async Task Main(string[] args)
        {
            var appConfig = InitOptions<AppConfig>();

            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read
            // regularly.

            _queueClient = new ServiceBusClient(appConfig.ConnectionStrings.AzureServiceBus);
            // create a processor that we can use to process the messages
            _processor = _queueClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            });

            try
            {
                // add handler to process messages
                _processor.ProcessMessageAsync += MessageHandlerAsync;

                // add handler to process any errors
                _processor.ProcessErrorAsync += ErrorHandlerAsync;

                // start processing 
                await _processor.StartProcessingAsync();

                Console.WriteLine("Wait while it is processing and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await _processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _processor.DisposeAsync();
                await _queueClient.DisposeAsync();
            }
        }

        private static async Task MessageHandlerAsync(ProcessMessageEventArgs args)
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Body);
            PersonModel person = JsonConvert.DeserializeObject<PersonModel>(jsonString);
            Console.WriteLine($"Person Received: {person.FirstName} {person.LastName}");

            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandlerAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler exception: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static T InitOptions<T>()
            where T : new()
        {
            var config = InitConfig();
            return config.Get<T>();
        }

        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

    }
}
