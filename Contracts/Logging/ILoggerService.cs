using GrowRoomEnvironment.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace GrowRoomEnvironment.Contracts.Logging
{
    public interface ILoggerService : IService
    {
        ILogger CreateLogger<T>();
    }
}
