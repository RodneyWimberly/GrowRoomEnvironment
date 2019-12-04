using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(DbContext context) : base(context)
        { }
    }
}
