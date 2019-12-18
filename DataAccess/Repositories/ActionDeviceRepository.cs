using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class ActionDeviceRepository : ApplicationDbRepository<ActionDevice>, IActionDeviceRepository
    {
        public ActionDeviceRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
