using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Models;


namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public class DataPointMapper : ITypeConverter<DataPointViewModel, DataPoint>
    {
        readonly IUnitOfWork _unitOfWork;
        public DataPointMapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DataPoint Convert(DataPointViewModel source, DataPoint destination, ResolutionContext context)
        {
            if (source == null)
                return null; ;

            if (destination == null)
            {
                destination = source.DataPointId > 0 ?
                    _unitOfWork.GetRepository<DataPoint>().Find(source.DataPointId) :
                    new DataPoint(source.DataPointId, source.Name, source.IsEnabled)
                    {
                        CreatedBy = source.CreatedBy,
                        CreatedDate = source.CreatedDate,
                        UpdatedBy = source.UpdatedBy,
                        UpdatedDate = source.UpdatedDate,
                        RowVersion = source.RowVersion
                    };
            }

            if (destination.DataPointId == source.DataPointId &&
                (destination.RowVersion == null || destination.RowVersion.IsEqualTo(source.RowVersion)))
            {
                if (destination.Name != source.Name)
                    destination.Name = source.Name;
                if (destination.IsEnabled != source.IsEnabled)
                    destination.IsEnabled = source.IsEnabled;
            }
            else
                throw new MappingConcurrencyException<DataPointViewModel, DataPoint>(source, destination);
            return destination;
        }
    }
}
