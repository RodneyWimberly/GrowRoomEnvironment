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
