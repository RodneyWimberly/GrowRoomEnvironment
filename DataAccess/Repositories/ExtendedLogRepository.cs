using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class ExtendedLogRepository : Repository<ExtendedLog>, IExtendedLogRepository
    {
        public ExtendedLogRepository(DbContext context) : base(context)
        { }

        public int ClearAll()
        {
            int records = 0;
            using (_context.Database.BeginTransaction())
            {
                records = _context.Database.ExecuteSqlRaw("Delete from AppLogs");
                _context.Database.CommitTransaction();
            }
            
            return records;
        }

        public async Task<int> ClearAllAsync()
        {
            int records = 0;
            using (await _context.Database.BeginTransactionAsync())
            {
                records = await _context.Database.ExecuteSqlRawAsync("Delete from AppLogs");
                _context.Database.CommitTransaction();
            }
            return records;
        }
    }
}
