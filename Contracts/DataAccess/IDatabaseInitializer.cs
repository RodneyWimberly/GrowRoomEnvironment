using System.Threading.Tasks;

namespace GrowRoomEnvironment.Contracts.DataAccess
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
}
