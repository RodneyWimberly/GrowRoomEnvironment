using GrowRoomEnvironment.Contracts.Services;

namespace GrowRoomEnvironment.Core.Services
{
    public abstract class ServiceBase : IService
    {
        public virtual void Startup()
        {
        }

    }
}
