using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpRequestExtensions
    {
        public async static Task<byte[]> ReadAsByteArrayAsync(this HttpRequest httpRequest)
        {
            using (var ms = new MemoryStream())
            using (var binaryReader = new BinaryReader(httpRequest.Body))
            {
                await ms.WriteAsync(binaryReader.ReadBytes((int)httpRequest.Body.Length), 0, (int)httpRequest.Body.Length);

                return ms.ToArray();
            }
        }

        public static string PathAndQuery(this HttpRequest httpRequest) => new UriBuilder(httpRequest.Scheme, httpRequest.Host.Host,
                                                                                          httpRequest.Host.Port ?? 80, httpRequest.Path,
                                                                                          httpRequest.QueryString.Value)
                                                                               .Uri.PathAndQuery;
    }
}
