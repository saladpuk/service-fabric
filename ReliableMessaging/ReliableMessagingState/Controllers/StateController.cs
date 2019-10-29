using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using ReliableMessaging.Shared;
using ReliableMessagingServer.Interfaces;
using ReliableMessagingState.Repository;

namespace ReliableMessagingState.Controllers
{
    [Route("api/[controller]")]
    public class StateController : Controller
    {
        private readonly IServerStateRepository stateRepository;

        public StateController(IServerStateRepository stateRepository)
        {
            this.stateRepository = stateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await stateRepository.Get();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var now = DateTime.Now;
            var all = await stateRepository.Get();
            if (all.Any() == false)
            {
                await stateRepository.Upsert(new ServerState { Id = 1, LastUpdatedDate = now, Status = "idle" });
            }
            else
            {
                var currentServerState = all.First();
                var actor = GetActor();
                var newServerState = await actor.AssignTaskAsync(currentServerState);
                await stateRepository.Upsert(newServerState);
            }
            return new OkResult();
        }

        private IReliableMessagingServer GetActor()
        {
            return ActorProxy.Create<IReliableMessagingServer>(
                ActorId.CreateRandom(),
                new Uri("fabric:/ReliableMessaging/ReliableMessagingServerActorService"));
        }
    }
}
