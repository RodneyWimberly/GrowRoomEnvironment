using GrowRoomEnvironment.Contracts.Logging;
using GrowRoomEnvironment.Core.Services;
using Microsoft.Extensions.Logging;
using System;

namespace GrowRoomEnvironment.Core.Logging
{
    public class LoggerService : ServiceBase, ILoggerService
    {

        ILoggerFactory _loggerFactory;

        public LoggerService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILogger CreateLogger<T>()
        {
            if (_loggerFactory == null)
            {
                throw new InvalidOperationException($"{nameof(_loggerFactory)} is not configured.");
            }

            return _loggerFactory.CreateLogger<T>();
        }
    }
}
