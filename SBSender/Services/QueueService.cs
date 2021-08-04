using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace SBSenderWeb.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _config;
        public QueueService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
        {
            var queueClient = new QueueClient(_config.GetConnectionString("AzureServiceBus"), queueName);
            string messageBody = JsonConvert.SerializeObject(serviceBusMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);

        }
    }
}
