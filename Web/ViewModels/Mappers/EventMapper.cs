using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Models;
using System.Collections.Generic;

namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public class EventMapper : ITypeConverter<EventViewModel, Event>
    {
        readonly IUnitOfWork _unitOfWork;
        public EventMapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Event Convert(EventViewModel source, Event destination, ResolutionContext context)
        {
            if (source == null)
                return null; ;

            if (destination == null)
            {
                destination = source.EventId > 0 ?
                    _unitOfWork.GetRepository<Event>().Find(source.EventId) :
                    new Event(source.EventId, source.Name, source.ActionDeviceId, source.State, source.IsEnabled)
                    {
                        ActionDevice = context.Mapper.Map<ActionDevice>(source.ActionDevice),
                        EventConditions = context.Mapper.Map<ICollection<EventCondition>>(source.EventConditions),
                        CreatedBy = source.CreatedBy,
                        CreatedDate = source.CreatedDate,
                        UpdatedBy = source.UpdatedBy,
                        UpdatedDate = source.UpdatedDate,
                        StateDescription = source.StateDescription,
                        RowVersion = source.RowVersion
                    };
            }

            if (destination.EventId == source.EventId &&
                (destination.RowVersion == null || destination.RowVersion.IsEqualTo(source.RowVersion)))
            {
                if (destination.Name != source.Name)
                    destination.Name = source.Name;
                if (destination.IsEnabled != source.IsEnabled)
                    destination.IsEnabled = source.IsEnabled;
                if (destination.ActionDeviceId != source.ActionDeviceId)
                {
                    destination.ActionDeviceId = source.ActionDeviceId;
                    destination.ActionDevice = context.Mapper.Map(source.ActionDevice, destination.ActionDevice);
                }
                if (destination.State != source.State)
                    destination.State = source.State;
                if (destination.StateDescription != source.StateDescription)
                    destination.StateDescription = source.StateDescription;

                destination.EventConditions = context.Mapper.Map(source.EventConditions, destination.EventConditions);
            }
            else
                throw new MappingConcurrencyException<EventViewModel, Event>(source, destination);
            return destination;
        }
    }
}
