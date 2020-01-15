using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using GrowRoomEnvironment.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;

namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public class EventConditionMapper : ITypeConverter<EventConditionViewModel, EventCondition>
    {
        readonly IUnitOfWork _unitOfWork;
        public EventConditionMapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public EventCondition Convert(EventConditionViewModel source, EventCondition destination, ResolutionContext context)
        {
            if (source == null)
                return null; ;

            if (destination == null)
            {
                destination = source.EventConditionId > 0 ?
                    _unitOfWork.GetRepository<EventCondition>().Find(source.EventConditionId) :
                    new EventCondition(source.EventConditionId, source.EventId, source.DataPointId, source.Operator, source.Value)
                    {
                        CreatedBy = source.CreatedBy,
                        CreatedDate = source.CreatedDate,
                        UpdatedBy = source.UpdatedBy,
                        UpdatedDate = source.UpdatedDate,
                        OperatorDescription = source.OperatorDescription,
                        DataPoint = context.Mapper.Map<DataPoint>(source.DataPoint),
                        Event = context.Mapper.Map<Event>(source.Event),
                        RowVersion = source.RowVersion
                    };
            }
            if (destination.EventConditionId == source.EventConditionId &&
                (destination.RowVersion == null || destination.RowVersion.IsEqualTo(source.RowVersion)))
            {
                if (destination.EventId != source.EventId)
                {
                    destination.EventId = source.EventId;
                    destination.Event = context.Mapper.Map<Event>(source.Event);
                }
                if (destination.DataPointId != source.DataPointId)
                {
                    destination.DataPointId = source.DataPointId;
                    destination.DataPoint = context.Mapper.Map<DataPoint>(source.DataPoint);

                }
                if (destination.Operator != source.Operator)
                    destination.Operator = source.Operator;
                if (destination.Value != source.Value)
                    destination.Value = source.Value;
                if (destination.OperatorDescription != source.OperatorDescription)
                    destination.OperatorDescription = source.OperatorDescription;
            }
            else
                throw new MappingConcurrencyException<EventConditionViewModel, EventCondition>(source, destination);
            return destination;
        }
    }
}
