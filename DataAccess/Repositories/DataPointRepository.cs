using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class DataPointRepository : Repository<DataPoint>, IDataPointRepository
    {
        public DataPointRepository(DbContext context) : base(context)
        { }
    }
}
