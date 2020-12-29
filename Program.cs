using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace httpclient_connection_pooling
{
    class Program
    {
        private async Task SimpleHttpClientTest()
        {
            // socketsHandler is default message handler used by HttpClient in .NET Core 2.1 and later.
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = 10,
                SslOptions = new SslClientAuthenticationOptions()
                {
                    EnabledSslProtocols = SslProtocols.Tls12,
                    RemoteCertificateValidationCallback = (a,b,c,d) => true
                }
            };

            // These static variables are no longer used.
            //  You were able to isolate static settings with AppDomain in the old days but AppDomain is not supported by asp.net core.
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;

            var client = new HttpClient(socketsHandler);
            var response = await client.GetAsync("https://www.google.com");
            Console.WriteLine(response.StatusCode);

            Console.ReadLine();
        }

        private async Task HttpClientConnectionPoolTest()
        {
            
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = 4,
                SslOptions = new SslClientAuthenticationOptions()
                {
                    EnabledSslProtocols = SslProtocols.Tls12,
                    RemoteCertificateValidationCallback = (a, b, c, d) => true
                }
            };

            var client = new HttpClient(socketsHandler);
            
            var tasks = Enumerable.Range(0, 20).Select(i => client.GetAsync("https://www.google.com"));
            
            await Task.WhenAll(tasks);

            Console.WriteLine("done");

            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Program p = new Program();

            Task.Run(() => p.HttpClientConnectionPoolTest());

            Console.ReadLine();
        }
    }
}
