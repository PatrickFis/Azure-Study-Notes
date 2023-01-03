using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace UdemyServiceBusQueueFunction
{
    public class ServiceBusQueue
    {
        [FunctionName("GetMessages")]
        public static void Run([ServiceBusTrigger("orderqueue", Connection = "connectionString")]Message QueueMessage, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {Encoding.UTF8.GetString(QueueMessage.Body)}");
            log.LogInformation($"Sequence Number {QueueMessage.SystemProperties.SequenceNumber}");
        }
    }
}
