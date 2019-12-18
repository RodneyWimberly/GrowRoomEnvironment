using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class DataPointRepository : ApplicationDbRepository<DataPoint>, IDataPointRepository
    {
        public DataPointRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
