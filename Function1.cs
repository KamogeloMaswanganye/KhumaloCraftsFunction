using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftsFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>(nameof(Invetory), "Inventory updated."));
            outputs.Add(await context.CallActivityAsync<string>(nameof(Payment), "Payment accepted."));
            outputs.Add(await context.CallActivityAsync<string>(nameof(Order), "Order confirmed."));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("Invetory")]
        public static string Invetory([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Upadting inentory in DB." + name);
            return $"{name}!";
        }

        [FunctionName("Payment")]
        public static string Payment([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Processing payment to DB." + name);
            return $"{name}!";
        }

        [FunctionName("Order")]
        public static string Order([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Confirming order in DB." + name);
            return $"{name}!";
        }

        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}