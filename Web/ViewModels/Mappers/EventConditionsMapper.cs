using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;


namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public class EventConditionsMapper : ITypeConverter<ICollection<EventConditionViewModel>, ICollection<EventCondition>>
    {
        readonly IUnitOfWork _unitOfWork;
        public EventConditionsMapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ICollection<EventCondition> Convert(ICollection<EventConditionViewModel> source, ICollection<EventCondition> destination, ResolutionContext context)
        {
            if (source == null)
                return null; ;

            if (destination == null)
            {
                destination = new List<EventCondition>();
            }
            if (destination.Count == 0 && source.Count > 0)
            {
                foreach (EventConditionViewModel sourceItem in source)
                {
                    EventCondition destinitionItem = _unitOfWork.GetRepository<EventCondition>().Find(sourceItem.EventConditionId);

                    destination.Add(destinitionItem != null ?
                        context.Mapper.Map(sourceItem, destinitionItem) :
                        context.Mapper.Map<EventCondition>(sourceItem));
                }
            }
            else
            {
                foreach (EventConditionViewModel sourceItem in source)
                {
                    EventCondition destinationItem = destination.Where(d => d.EventConditionId == sourceItem.EventConditionId).SingleOrDefault();
                    if (destinationItem == null)
                    {
                        destinationItem = _unitOfWork.GetRepository<EventCondition>().GetFirstOrDefault(ec => ec, ec => ec.EventConditionId == sourceItem.EventConditionId);
                        destination.Add(destinationItem);
                    }
                    destinationItem = context.Mapper.Map(sourceItem, destinationItem);
                }
            }
            return destination;
        }
    }
}
