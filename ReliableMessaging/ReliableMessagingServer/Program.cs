using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using LeftPaddle.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using RightPaddle.Interfaces;

namespace ReliableMessagingServer
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform

                var leftPaddle = ActorProxy.Create<ILeftPaddle>(
                        new ActorId("Left"),
                        new Uri("fabric:/ReliableMessaging/LeftPaddleActorService"));

                var rightPaddle = ActorProxy.Create<IRightPaddle>(
                        new ActorId("Right"),
                        new Uri("fabric:/ReliableMessaging/RightPaddleActorService"));

                ActorRuntime.RegisterActorAsync<ReliableMessagingServer>(
                           (context, actorType) => new ActorService(context, actorType, (actSvc, actId) => new ReliableMessagingServer(actSvc, actId, leftPaddle, rightPaddle)))
                    .GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
