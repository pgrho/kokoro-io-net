using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shipwreck.KokoroIO
{
    [Obsolete("Use KokoroIO.Client instead.", true)]
    public static class HttpRequestMessageExtensions
    {
        [Obsolete("Use KokoroIO.Client instead.", true)]
        public static Task<Message> GetMessageAsync(this HttpRequestMessage request, string callbackSecret)
            => throw new NotSupportedException();
    }
}