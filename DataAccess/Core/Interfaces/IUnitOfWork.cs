using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;

namespace GrowRoomEnvironment.DataAccess.Core.Interfaces
{
    public interface IUnitOfWork
    {

        IEnumLookupRespository EnumLookups { get; }

        IDataPointRepository DataPoints { get; }

        int SaveChanges();
    }
}
