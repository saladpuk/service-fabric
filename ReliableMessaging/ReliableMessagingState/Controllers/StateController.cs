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
            var serverState = await stateRepository.Get();
            if (serverState == null)
            {
                await stateRepository.Upsert(new ServerState { IsLeftServerSending = true, IsServerActive = true });
            }
            return Json(serverState);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var serverState = await stateRepository.Get();
            if (serverState == null)
            {
                return new OkResult();
            }

            try
            {
                var actor = GetActor();
                serverState = await actor.AssignTaskAsync(serverState);
                serverState.IsServerActive = true;
            }
            catch (Exception)
            {
                serverState.IsServerActive = false;
            }
            finally
            {
                await stateRepository.Upsert(serverState);
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
