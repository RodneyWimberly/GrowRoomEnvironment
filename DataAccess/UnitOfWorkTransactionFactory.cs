using System.Threading;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWorkTransactionFactory : IUnitOfWork
    {
        public int AutoDisposeTimerInterval { get; set; } = 1500;
        public Timer DisposeTimer { get; set; }
        public DbContextOptions<ApplicationDbContext> DbContextOptions { get; set; }
        public ApplicationDbContext DbContext { get; set; }

        public UnitOfWorkTransactionFactory(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            DbContextOptions = dbContextOptions;
        }
            
        public IEnumLookupRespository EnumLookups
        {
            get
            {
                return new EnumLookupRespository(CreateDbContext());
            }
        }          
        
        public IDataPointRepository DataPoints
        {
            get
            {
                return new DataPointRepository(CreateDbContext());
            }
        }

        public IExtendedLogRepository ExtendedLogs
        {
            get
            {
                IExtendedLogRepository repository = new ExtendedLogRepository(CreateDbContext());
                return repository;
            }
        }

        public IActionDeviceRepository ActionDevices
        {
            get
            {
                return new ActionDeviceRepository(CreateDbContext());
            }
        }

        public IEventRepository Events
        {
            get
            {
                return new EventRepository(CreateDbContext());
            }
        }

        public IEventConditionRepository EventConditions
        {
            get
            {
                return new EventConditionRepository(CreateDbContext());
            }
        }

        public INotificationRepository Notifications
        {
            get
            {
                return new NotificationRepository(CreateDbContext());
            }
        }

        public int SaveChanges()
        {
            CreateDbContext();
            int returnValue = DbContext.SaveChanges();
            DisposeDbContext();
            return returnValue;
        }

        public async Task<int> SaveChangesAsync()
        {
            CreateDbContext();
            int returnValue = await DbContext.SaveChangesAsync();
            DisposeDbContext();
            return returnValue;
        }

        private ApplicationDbContext CreateDbContext()
        {
            if (DbContext == null)
            {
                if(AutoDisposeTimerInterval > 0)
                    DisposeTimer = new Timer(state => DisposeDbContext(), null, AutoDisposeTimerInterval, Timeout.Infinite);
                DbContext = new ApplicationDbContext(DbContextOptions);
            }

            return DbContext;
        }

        private void DisposeDbContext()
        {
            if (DisposeTimer != null)
            {
                DisposeTimer.Dispose();
                DisposeTimer = null;
            }
            if (DbContext != null)
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}
