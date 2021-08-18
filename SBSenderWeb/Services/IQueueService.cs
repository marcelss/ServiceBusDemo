using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace SBSenderWeb.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string topicOrQueueName);

        Task SendMessagesAsync<T>(IList<T> serviceBusMessages, string topicOrQueueName);
        
    }
}