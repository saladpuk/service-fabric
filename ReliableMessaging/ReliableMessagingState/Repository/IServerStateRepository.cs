using ReliableMessaging.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReliableMessagingState.Repository
{
    public interface IServerStateRepository
    {
        Task<IEnumerable<ServerState>> Get();
        Task Upsert(ServerState serverState);
    }
}
