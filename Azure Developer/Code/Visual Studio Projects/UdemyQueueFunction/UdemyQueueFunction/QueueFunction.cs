using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace UdemyQueueFunction
{
    public class QueueFunction
    {
        [FunctionName("QueueFunction")]
        public void Run([QueueTrigger("appqueue", Connection = "connectionString")]Order order, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: Order ID: {order.Id}. Order quantity: {order.Quantity}.");
        }
    }
}
