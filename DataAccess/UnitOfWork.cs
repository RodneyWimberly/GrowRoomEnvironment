using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext Context;

        IEnumLookupRespository _enumLookups;
        IDataPointRepository _dataPoints;
        IExtendedLogRepository _extendedLogs;
        IActionDeviceRepository _actionDevices;
        IEventRepository _events;
        IEventConditionRepository _eventConditions;
        INotificationRepository _notifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumLookupRespository EnumLookups
        {
            get
            {
                if (_enumLookups == null)
                    _enumLookups = new EnumLookupRespository(Context);

                return _enumLookups;
            }
        }

        public IDataPointRepository DataPoints
        {
            get
            {
                if (_dataPoints == null)
                    _dataPoints = new DataPointRepository(Context);

                return _dataPoints;
            }
        }

        public IExtendedLogRepository ExtendedLogs
        {
            get
            {
                if (_extendedLogs == null)
                    _extendedLogs = new ExtendedLogRepository(Context);

                return _extendedLogs;
            }
        }

        public IActionDeviceRepository ActionDevices
        {
            get
            {
                if (_actionDevices == null)
                    _actionDevices = new ActionDeviceRepository(Context);

                return _actionDevices;
            }
        }

        public IEventRepository Events
        {
            get
            {
                if (_events == null)
                    _events = new EventRepository(Context);

                return _events;
            }
        }

        public IEventConditionRepository EventConditions
        {
            get
            {
                if (_eventConditions == null)
                    _eventConditions = new EventConditionRepository(Context);

                return _eventConditions;
            }
        }

        public INotificationRepository Notifications
        {
            get
            {
                if (_notifications == null)
                    _notifications = new NotificationRepository(Context);

                return _notifications;
            }
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
