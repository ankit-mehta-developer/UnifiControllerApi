using System.Linq;
using System.Threading.Tasks;

namespace UnifiControllerCommunicator.Helpers
{
    public static class UnifiControllerExtensions
    {
        public static async Task<NetworkClient[]> GetNetworkClients(this IUnifiControllerApi controller)
        {
            var clients = (await controller.GetClients()).Select(c => new NetworkClient
            {
                Name = (c.GetValueOrDefault("name") ?? c.GetValueOrDefault("hostname") ?? c.GetValueOrDefault("oui") ?? c.GetValueOrDefault("mac"))?.ToString(),
                Signal = c.GetValueOrDefault("signal")?.ToString().ToNullableDecimal(),
                IdleTime = c.GetValueOrDefault("idletime")?.ToString().ToNullableDecimal(),
                LastSeen = c.GetValueOrDefault("last_seen")?.ToString().ToNullableLong().ToDateTimeFromUnixTicks(),
            });
            return clients.ToArray();
        }
    }
}
