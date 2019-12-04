using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        IEnumLookupRespository _enumLookups;
        IDataPointRepository _dataPoints;
        IExtendedLogRepository _extendedLogs;
        IActionDeviceRepository _actionDevices;
        IEventRepository _events;
        IEventConditionRepository _eventConditions;
        INotificationRepository _notifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumLookupRespository EnumLookups
        {
            get
            {
                if (_enumLookups == null)
                    _enumLookups = new EnumLookupRespository(_context);

                return _enumLookups;
            }
        }

        public IDataPointRepository DataPoints
        {
            get
            {
                if (_dataPoints == null)
                    _dataPoints = new DataPointRepository(_context);

                return _dataPoints;
            }
        }

        public IExtendedLogRepository ExtendedLogs
        {
            get
            {
                if (_extendedLogs == null)
                    _extendedLogs = new ExtendedLogRepository(_context);

                return _extendedLogs;
            }
        }

        public IActionDeviceRepository ActionDevices
        {
            get
            {
                if (_actionDevices == null)
                    _actionDevices = new ActionDeviceRepository(_context);

                return _actionDevices;
            }
        }

        public IEventRepository Events
        {
            get
            {
                if (_events == null)
                    _events = new EventRepository(_context);

                return _events;
            }
        }

        public IEventConditionRepository EventConditions
        {
            get
            {
                if (_eventConditions == null)
                    _eventConditions = new EventConditionRepository(_context);

                return _eventConditions;
            }
        }

        public INotificationRepository Notifications
        {
            get
            {
                if (_notifications == null)
                    _notifications = new NotificationRepository(_context);

                return _notifications;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
