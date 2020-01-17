using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Models;

namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public class ActionDeviceMapper : ITypeConverter<ActionDeviceViewModel, ActionDevice>
    {
        readonly IUnitOfWork _unitOfWork;
        public ActionDeviceMapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionDevice Convert(ActionDeviceViewModel source, ActionDevice destination, ResolutionContext context)
        {
            if (source == null)
                return null;

            if (destination == null)
            {
                destination = source.ActionDeviceId > 0 ?
                    _unitOfWork.GetRepository<ActionDevice>().GetFirstOrDefault(predicate: ad => ad.ActionDeviceId == source.ActionDeviceId) :
                    new ActionDevice(source.ActionDeviceId, source.Name, source.Type, source.Parameters, source.IsEnabled)
                    {
                        CreatedBy = source.CreatedBy,
                        CreatedDate = source.CreatedDate,
                        UpdatedBy = source.UpdatedBy,
                        UpdatedDate = source.UpdatedDate,
                        TypeDescription = source.TypeDescription,
                        RowVersion = source.RowVersion
                    };
            }


            if (destination.ActionDeviceId == source.ActionDeviceId &&
                 (destination.RowVersion == null || destination.RowVersion.IsEqualTo(source.RowVersion)))
            {
                if (destination.Name != source.Name)
                    destination.Name = source.Name;
                if (destination.IsEnabled != source.IsEnabled)
                    destination.IsEnabled = source.IsEnabled;
                if (destination.Type != source.Type)
                    destination.Type = source.Type;
                if (destination.Parameters != source.Parameters)
                    destination.Parameters = source.Parameters;
                if (destination.TypeDescription != source.TypeDescription)
                    destination.TypeDescription = source.TypeDescription;
            }
            else
                throw new MappingConcurrencyException<ActionDeviceViewModel, ActionDevice>(source, destination);

            return destination;
        }
    }
}
