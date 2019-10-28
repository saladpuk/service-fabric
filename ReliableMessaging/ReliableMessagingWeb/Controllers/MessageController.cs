using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ReliableMessagingWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Message")]
    public class MessageController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public MessageController(HttpClient httpClient, StatelessServiceContext context, FabricClient fabricClient)
        {
            this.fabricClient = fabricClient;
            this.httpClient = httpClient;
            this.serviceContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var serviceName = ReliableMessagingWeb.GetReliableMessagingStateServiceName(this.serviceContext);
            var proxyAddress = this.GetProxyAddress(serviceName);
            var partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(serviceName);
            var result = new List<KeyValuePair<string, string>>();

            foreach (Partition partition in partitions)
            {
                var proxyUrl = $"{proxyAddress}/api/State?PartitionKey={((Int64RangePartitionInformation)partition.PartitionInformation).LowKey}&PartitionKind=Int64Range";

                using (HttpResponseMessage response = await this.httpClient.GetAsync(proxyUrl))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        continue;
                    }

                    var responseText = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(responseText);
                    result.AddRange(data);
                }
            }

            var qry = result.Select(it => new
            {
                Name = it.Key,
                Status = it.Value
            });
            return this.Json(qry);
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}
