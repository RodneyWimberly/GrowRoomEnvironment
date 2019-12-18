using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class EventConditionRepository : ApplicationDbRepository<EventCondition>, IEventConditionRepository
    {
        public EventConditionRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
