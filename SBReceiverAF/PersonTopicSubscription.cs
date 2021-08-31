using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SBReceiverAF
{
    public static class PersonTopicSubscription
    {
        [FunctionName("PersonTopicSubscription")]
        public static void Run([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")]Message message, MessageReceiver messageReceiver, ILogger log)
        {
            try
            {
                string body = Encoding.UTF8.GetString(message.Body);
                Console.WriteLine($"Received: {body} from persontopic");
                log.LogInformation($"C# ServiceBus topic trigger function processed message: {body}");

                PlannedOrder myDeserializedClass = JsonConvert.DeserializeObject<PlannedOrder>(body);
                //Do your things here, such as Some Actions or Calls Some Service or Another Method

                messageReceiver.AbandonAsync(message.SystemProperties.LockToken);

                //complete the message if there is no error
                messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

            }
            catch (Exception)
            {
                // Do your error handling here


                // Send message to DeadLetter Queue 
                messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken);

            }
        }
    }
}
