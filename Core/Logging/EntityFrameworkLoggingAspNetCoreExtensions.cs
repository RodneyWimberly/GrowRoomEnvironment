using Microsoft.AspNetCore.Builder;


namespace GrowRoomEnvironment.Core.Logging
{
    public static class EntityFrameworkLoggingAspNetCoreExtensions
    {
        public static IApplicationBuilder UseEntityFrameworkLoggingScopeStateProvider(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EntityFrameworkLoggingScopeStateProviderMiddleware>();
        }

    }
}
