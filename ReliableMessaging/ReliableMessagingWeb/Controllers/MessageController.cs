﻿using System;
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
using ReliableMessaging.Shared;

namespace ReliableMessagingWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
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
            var serviceName = ReliableMessagingWeb.GetReliableMessagingStateServiceName(serviceContext);
            var proxyAddress = GetProxyAddress(serviceName);
            var partitions = await fabricClient.QueryManager.GetPartitionListAsync(serviceName);
            ServerState data = null;

            foreach (Partition partition in partitions)
            {
                var proxyUrl = $"{proxyAddress}/api/State?PartitionKey={((Int64RangePartitionInformation)partition.PartitionInformation).LowKey}&PartitionKind=Int64Range";

                using var response = await httpClient.GetAsync(proxyUrl);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    continue;
                }

                var responseText = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<ServerState>(responseText);
            }

            var sendingState = data.IsLeftServerSending ? "left2right" : "right2left";
            var svStatus = data.IsServerActive ? string.Empty : "down";
            return Json(new
            {
                ServerStatus = $"{sendingState}{svStatus}"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var serviceName = ReliableMessagingWeb.GetReliableMessagingStateServiceName(serviceContext);
            var proxyAddress = GetProxyAddress(serviceName);
            var stupidKey = Guid.NewGuid().ToString();
            var partitionKey = GetPartitionKey(stupidKey);
            var proxyUrl = $"{proxyAddress}/api/State?PartitionKey={partitionKey}&PartitionKind=Int64Range";

            using HttpResponseMessage response = await httpClient.PostAsync(proxyUrl, null);
            return new ContentResult()
            {
                StatusCode = (int)response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };
        }

        private Uri GetProxyAddress(Uri serviceName)
            => new Uri($"http://localhost:19081{serviceName.AbsolutePath}");

        private long GetPartitionKey(string name)
            => Char.ToUpper(name.First()) - 'A';
    }
}
