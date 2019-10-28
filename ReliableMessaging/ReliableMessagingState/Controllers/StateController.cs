using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;

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
            var result = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1", "sending"),
                new KeyValuePair<string, string>("2", "sendingdown"),
                new KeyValuePair<string, string>("3", "idle"),
                new KeyValuePair<string, string>("4", "idledown"),
            };
            return this.Json(result);
        }
    }
}
