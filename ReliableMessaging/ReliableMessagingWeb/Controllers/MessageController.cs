using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LeftPaddle.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json;
using ReliableMessaging.Shared;
using ReliableMessagingServer.Interfaces;
using RightPaddle.Interfaces;

namespace ReliableMessagingWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private ILeftPaddle leftPaddle = ActorProxy.Create<ILeftPaddle>(
                new ActorId("Left"),
                new Uri("fabric:/ReliableMessaging/LeftPaddleActorService"));

        private IRightPaddle rightPaddle = ActorProxy.Create<IRightPaddle>(
                new ActorId("Right"),
                new Uri("fabric:/ReliableMessaging/RightPaddleActorService"));

        [HttpGet("{ballY}/{positionY}/{height}/{tag}")]
        public async Task<IActionResult> Get(double ballY, double positionY, double height, string tag)
        {
            if (tag == "left")
            {
                var actor = leftPaddle;
                var result = await actor.CalculatePosition(new CalculationRequest
                {
                    BallPositionY = ballY,
                    PlayerPositionY = positionY,
                    PlayerHeight = height,
                });
                return Json(result.MovementPoistionY);
            }
            else
            {
                var actor = rightPaddle;
                var result = await actor.CalculatePosition(new CalculationRequest
                {
                    BallPositionY = ballY,
                    PlayerPositionY = positionY,
                    PlayerHeight = height,
                });
                return Json(result.MovementPoistionY);
            }
        }
    }
}
