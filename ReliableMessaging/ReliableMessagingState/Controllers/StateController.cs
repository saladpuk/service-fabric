using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using ReliableMessaging.Shared;
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
            await stateRepository.Create(new ServerState { Id = 1, LastUpdatedDate = now, Status = "idle" });
            await stateRepository.Create(new ServerState { Id = 2, LastUpdatedDate = now, Status = "idle" });
            return new OkResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            await stateRepository.Update(id);
            return new OkResult();
        }
    }
}
