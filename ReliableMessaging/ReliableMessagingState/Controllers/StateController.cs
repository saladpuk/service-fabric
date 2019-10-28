using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ReliableMessagingState.Controllers
{
    [Route("api/[controller]")]
    public class StateController : Controller
    {
        private readonly IReliableStateManager stateManager;

        public StateController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ct = new CancellationToken();
            var serverStatusDictionary = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, string>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            var list = await serverStatusDictionary.CreateEnumerableAsync(tx);
            var enumerator = list.GetAsyncEnumerator();
            var result = new List<KeyValuePair<int, string>>();

            while (await enumerator.MoveNextAsync(ct))
            {
                result.Add(enumerator.Current);
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var serverStatusDictionary = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, string>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            await serverStatusDictionary.AddAsync(tx, 1, "sending");
            await serverStatusDictionary.AddAsync(tx, 2, "idle");
            await serverStatusDictionary.AddAsync(tx, 3, "idle");
            await serverStatusDictionary.AddAsync(tx, 4, "idle");
            await tx.CommitAsync();

            return new OkResult();
        }
    }
}
