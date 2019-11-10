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
        private readonly IReliableMessagingServer actor;

        public MessageController(IReliableMessagingServer actor)
        {
            this.actor = actor;
        }

        [HttpGet("{ballY}/{positionY}/{height}/{tag}")]
        public async Task<IActionResult> Get(double ballY, double positionY, double height, string tag)
        {
            var result = await actor.GetPaddlePosition(new GameInformation
            {
                BallPositionY = ballY,
                PlayerPositionY = positionY,
                PlayerHeight = height,
                Tag = tag,
            });
            return Json(result.MovementPoistionY);
        }
    }
}
