using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using ReliableMessaging.Shared;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace ReliableMessagingServer.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IReliableMessagingServer : IActor
    {
        Task<ServerState> AssignTaskAsync(ServerState serverState);
        Task<CalculationResult> Calculate(double ballY, double positionY, double height);
    }

    public class CalculationResult
    {
        public double Y { get; set; }
    }
}
