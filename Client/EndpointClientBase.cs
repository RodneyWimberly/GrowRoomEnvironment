using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Client
{
    public abstract class EndpointClientBase
    {
        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                return httpRequestMessage;
            }, cancellationToken);
        }
    }
}
