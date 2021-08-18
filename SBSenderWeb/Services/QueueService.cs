using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SBSenderWeb.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _config;
        // the client that owns the connection and can be used to create senders and receivers
        private ServiceBusClient _queueClient;

        // the sender used to publish messages to the queue
        private ServiceBusSender _sender;
        public QueueService(IConfiguration config, ServiceBusClient queueClient)
        {
            _queueClient = queueClient;
            _config = config;
        }

        public async Task SendMessageAsync<T>(T serviceBusMessage, string topicOrQueueName)
        {
            try
            {
                _sender = _queueClient.CreateSender(topicOrQueueName);

                string messageBody = JsonConvert.SerializeObject(serviceBusMessage);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                message.ApplicationProperties["type"] = typeof(T).Name;
                await _sender.SendMessageAsync(message);
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _sender.DisposeAsync();
            }
        }

        public async Task SendMessagesAsync<T>(IList<T> serviceBusMessages, string topicOrQueueName)
        {
            _sender = _queueClient.CreateSender(topicOrQueueName);

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();

            foreach (var serviceBusMessage in serviceBusMessages)
            {
                string messageBody = JsonConvert.SerializeObject(serviceBusMessage);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                message.ApplicationProperties["type"] = typeof(T).Name;
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(message))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {message} is too large to fit in the batch.");
                }
            }

            try
            {
                await _sender.SendMessagesAsync(messageBatch);
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _sender.DisposeAsync();
            }
        }

    }
}
