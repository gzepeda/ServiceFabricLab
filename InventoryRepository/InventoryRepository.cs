using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric.Description;
using InventoryRepository.ServiceFabric;

namespace InventoryRepository
{
    /// <summary>
    /// El runtime de Service Fabric crea una instancia de esta clase para cada réplica de servicio.
    /// </summary>
    internal sealed class InventoryRepository : StatefulService
    {
        public InventoryRepository(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Reemplazo opcional para crear escuchas (por ejemplo, HTTP, comunicación remota del servicio, WCF, etc.) de forma que esta réplica del servicio controle las solicitudes de cliente o de usuario.
        /// </summary>
        /// <remarks>
        /// Para obtener más información sobre la comunicación entre servicios, vea https://aka.ms/servicefabricservicecommunication.
        /// </remarks>
        /// <returns>Una colección de agentes de escucha.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            var endpoints = Context.CodePackageActivationContext.GetEndpoints()
                       .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https)
                       .Select(endpoint => endpoint.Name);

            return endpoints.Select(
                endpoint => new ServiceReplicaListener(
                    serviceContext => new OwinCommunicationListener(
                        appBuilder => { WebHostStartup.ConfigureApp(appBuilder, StateManager); },
                        serviceContext,
                        ServiceEventSource.Current,
                        endpoint),
                    endpoint));
        }

        /// <summary>
        /// Este es el punto de entrada principal para la réplica del servicio.
        /// Este método se ejecuta cuando esta réplica del servicio pasa a ser principal y tiene estado de escritura.
        /// </summary>
        /// <param name="cancellationToken">Se cancela cuando Service Fabric tiene que cerrar esta réplica del servicio.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Reemplace el siguiente código de ejemplo por su propia lógica 
        //    //       o quite este reemplazo de RunAsync si no es necesario en su servicio.

        //    var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        using (var tx = this.StateManager.CreateTransaction())
        //        {
        //            var result = await myDictionary.TryGetValueAsync(tx, "Counter");

        //            ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
        //                result.HasValue ? result.Value.ToString() : "Value does not exist.");

        //            await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

        //            // Si se produce una excepción antes de llamar a CommitAsync, se anula la transacción, se descartan todos los cambios
        //            // y no se guarda nada en las réplicas secundarias.
        //            await tx.CommitAsync();
        //        }

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}
