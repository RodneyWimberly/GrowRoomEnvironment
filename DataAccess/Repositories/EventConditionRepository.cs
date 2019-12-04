using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class EventConditionRepository : Repository<EventCondition>, IEventConditionRepository
    {
        public EventConditionRepository(DbContext context) : base(context)
        { }
    }
}
