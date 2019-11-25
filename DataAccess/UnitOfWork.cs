using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        IEnumLookupRespository _enumLookups;
        IDataPointRepository _dataPoints;

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
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
