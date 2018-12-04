using System;
using System.Net;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TodoAPI.HttpRequest
{
    public class HttpGet : IHttpGet
    {
        private readonly ILogger _logger;

        public HttpGet(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetFromURLAsync(string uri)
        {
            _logger.LogDebug(String.Format("Getting data from URL: {0}", uri));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                _logger.LogDebug(String.Format("Done getting data from URL: {0}", uri));
                if ((int)response.StatusCode != 200)
                {
                    throw new Exception(reader.ReadToEnd());
                }
                return reader.ReadToEnd();
            }
        }
    }
}
