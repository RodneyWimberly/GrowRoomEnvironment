using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext Context;

        private IDataPointRepository _dataPoints;
        private IExtendedLogRepository _extendedLogs;
        private IActionDeviceRepository _actionDevices;
        private IEventRepository _events;
        private IEventConditionRepository _eventConditions;
        private INotificationRepository _notifications;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            Context = context;
            //BeginTransaction();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Context.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Context.Database.RollbackTransaction();
        }

        public void DetachAll()
        {
            Context.DetachAllEntities();
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose repositories
                    if (_dataPoints != null)
                        _dataPoints = null;
                    if (_extendedLogs != null)
                        _extendedLogs = null;
                    if (_actionDevices != null)
                        _actionDevices = null;
                    if (_events != null)
                        _events = null;
                    if (_eventConditions != null)
                        _eventConditions = null;
                    if (_notifications != null)
                        _notifications = null;

                    // dispose the db context.
                    Context.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
