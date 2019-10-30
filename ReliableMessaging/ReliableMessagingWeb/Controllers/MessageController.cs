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
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json;
using ReliableMessagingServer.Interfaces;

namespace ReliableMessagingWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        [HttpGet("{ballY}/{positionY}/{height}")]
        public async Task<IActionResult> Get(double ballY, double positionY, double height)
        {
            var actor = GetActor();
            var result = await actor.CalculatePosition(new CalculationRequest
            {
                BallPositionY = ballY,
                PlayerPositionY = positionY,
                PlayerHeight = height
            });
            return Json(result.MovementPoistionY);
        }

        private IReliableMessagingServer GetActor()
        {
            return ActorProxy.Create<IReliableMessagingServer>(
                ActorId.CreateRandom(),
                new Uri("fabric:/ReliableMessaging/ReliableMessagingServerActorService"));
        }
    }
}
