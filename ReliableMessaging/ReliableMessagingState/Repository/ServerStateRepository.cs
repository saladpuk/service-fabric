using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using ReliableMessaging.Shared;

namespace ReliableMessagingState.Repository
{
    public class ServerStateRepository : IServerStateRepository
    {
        private readonly IReliableStateManager stateManager;

        public ServerStateRepository(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        #region IServerStateRepository

        public async Task<ServerState> Get()
        {
            var ct = new CancellationToken();
            var serverStatusDictionary = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, ServerState>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            var list = await serverStatusDictionary.CreateEnumerableAsync(tx);
            var enumerator = list.GetAsyncEnumerator();
            var result = new List<KeyValuePair<int, ServerState>>();

            while (await enumerator.MoveNextAsync(ct))
            {
                result.Add(enumerator.Current);
            }

            return result.Select(it => it.Value).FirstOrDefault();
        }

        public async Task Upsert(ServerState serverState)
        {
            var serverStatusDictionary = await stateManager
               .GetOrAddAsync<IReliableDictionary<int, ServerState>>("serverStatus");

            using var tx = stateManager.CreateTransaction();
            await serverStatusDictionary.AddOrUpdateAsync(tx, 1, serverState, (key, old) => serverState);
            await tx.CommitAsync();
        }

        #endregion IServerStateRepository
    }
}
