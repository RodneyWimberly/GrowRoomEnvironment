using GrowRoomEnvironment.Contracts.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Repositories.Interfaces
{
    public interface IExtendedLogRepository : IRepository<ExtendedLog>
    {
        int ClearAll();
        Task<int> ClearAllAsync();
    }
}
