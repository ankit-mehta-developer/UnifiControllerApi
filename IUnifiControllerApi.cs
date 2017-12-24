using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnifiControllerCommunicator
{
    public interface IUnifiControllerApi
    {
        Task<bool> Login();
        Task LogOut();
        Task<IDictionary<string, object>[]> GetClients();
        bool IsLoggedIn { get; }
    }
}
