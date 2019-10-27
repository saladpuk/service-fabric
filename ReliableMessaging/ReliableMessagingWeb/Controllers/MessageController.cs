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
            var random = new Random();
            var votes = new List<KeyValuePair<string, int>>();
            votes.Add(new KeyValuePair<string, int>("Pizza", random.Next(1, 100)));
            votes.Add(new KeyValuePair<string, int>("Ice cream", random.Next(1, 100)));
            return Json(votes);
        }
    }
}
