using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IDataPointRepository DataPoints { get; }

        IExtendedLogRepository ExtendedLogs { get; }

        IActionDeviceRepository ActionDevices { get; }

        IEventRepository Events { get; }

        IEventConditionRepository EventConditions { get; }

        INotificationRepository Notifications { get; }

        void DetachAll();

        IDbContextTransaction BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}
