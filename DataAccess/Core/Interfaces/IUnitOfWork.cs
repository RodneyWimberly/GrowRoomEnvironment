using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IEnumLookupRespository EnumLookups { get; }

        IDataPointRepository DataPoints { get; }

        IExtendedLogRepository ExtendedLogs { get; }

        IActionDeviceRepository ActionDevices { get; }

        IEventRepository Events { get; }

        IEventConditionRepository EventConditions { get; }

        INotificationRepository Notifications { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}
