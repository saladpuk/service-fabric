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
            var votesDictionary = await stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            var list = await votesDictionary.CreateEnumerableAsync(tx);
            var enumerator = list.GetAsyncEnumerator();
            var result = new List<KeyValuePair<string, string>>();

            while (await enumerator.MoveNextAsync(ct))
            {
                result.Add(enumerator.Current);
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var votesDictionary = await stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            await votesDictionary.AddOrUpdateAsync(tx, "1", "sending", (key, oldvalue) => "sendingdown");
            await votesDictionary.AddOrUpdateAsync(tx, "2", "sendingdown", (key, oldvalue) => "sending");
            await votesDictionary.AddOrUpdateAsync(tx, "3", "idle", (key, oldvalue) => "sending");
            await votesDictionary.AddOrUpdateAsync(tx, "4", "idledown", (key, oldvalue) => "idle");
            await tx.CommitAsync();

            return new OkResult();
        }
    }
}
