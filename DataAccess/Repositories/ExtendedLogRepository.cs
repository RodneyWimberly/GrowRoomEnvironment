using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class ExtendedLogRepository : Repository<ExtendedLog>, IExtendedLogRepository
    {
        public ExtendedLogRepository(DbContext context) : base(context)
        { }
    }
}
