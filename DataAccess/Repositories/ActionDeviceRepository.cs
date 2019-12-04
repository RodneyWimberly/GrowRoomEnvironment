using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class ActionDeviceRepository : Repository<ActionDevice>, IActionDeviceRepository
    {
        public ActionDeviceRepository(DbContext context) : base(context)
        { }
    }
}
