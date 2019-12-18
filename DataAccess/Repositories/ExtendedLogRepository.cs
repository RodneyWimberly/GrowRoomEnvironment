using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class ExtendedLogRepository : ApplicationDbRepository<ExtendedLog>, IExtendedLogRepository
    {
        public ExtendedLogRepository(ApplicationDbContext context) : base(context)
        { }

        public int ClearAll()
        {
            int records = 0;
            using (Database.BeginTransaction())
            {
                records = Context.Database.ExecuteSqlRaw("Delete from AppLogs");
                Context.Database.CommitTransaction();
            }
            
            return records;
        }

        public async Task<int> ClearAllAsync()
        {
            int records = 0;
            using (await Database.BeginTransactionAsync())
            {
                records = await Context.Database.ExecuteSqlRawAsync("Delete from AppLogs");
                Context.Database.CommitTransaction();
            }
            return records;
        }
    }
}
