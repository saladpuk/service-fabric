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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new[]
            {
                new { Name = "1",  Status = "sending" },
                new { Name = "2",  Status = "sendingdown" },
                new { Name = "3",  Status = "idle" },
                new { Name = "4",  Status = "idledown" },
            };
            return this.Json(result);
        }
    }
}
