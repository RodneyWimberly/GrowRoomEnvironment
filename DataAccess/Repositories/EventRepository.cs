using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class EventRepository : ApplicationDbRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        { }

        public override IQueryable<Event> Find(Expression<Func<Event, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            IIncludableQueryable<Event, DataPoint> events = base.Find(predicate, pageNumber, pageSize)
                .Include(e => e.ActionDevice)
                .Include(e => e.EventConditions)
                .ThenInclude(ec => ec.DataPoint);
            return events;
        }

        public override async Task<IList<Event>> FindAsync(Expression<Func<Event, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            List<Event> events = await Find(predicate, pageNumber, pageSize).ToListAsync();
            return events;
        }

        public override IQueryable<Event> GetAll(int pageNumber = -1, int pageSize = -1)
        {
            IIncludableQueryable<Event, Event> events = base.GetAll(pageNumber, pageSize)
                .Include(e => e.ActionDevice)
                .Include(e => e.EventConditions)
                    .ThenInclude(ec => ec.DataPoint)
                .Include(e => e.EventConditions)
                    .ThenInclude(ec => ec.Event);
            return events;
        }

        public override async Task<IList<Event>> GetAllAsync(int pageNumber = -1, int pageSize = -1)
        {
            List<Event> events = await GetAll(pageNumber, pageSize).ToListAsync();
            return events;
        }

        public override Event Get(int id)
        {
            Event @event= base.Get(id);
            Context.Entry(@event).Reference(e => e.ActionDevice).Load();
            Context.Entry(@event).Collection(e => e.EventConditions).Query()
                .Include(ec => ec.DataPoint).Load();
            return @event;
        }

        public override async Task<Event> GetAsync(int id)
        {
            Event @event = await base.GetAsync(id);
            await Context.Entry(@event).Reference(e => e.ActionDevice).LoadAsync();
            await Context.Entry(@event).Collection(e => e.EventConditions).Query()
                .Include(ec => ec.DataPoint).LoadAsync();
            return @event;
        }
    }
}
