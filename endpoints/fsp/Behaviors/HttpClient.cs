using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace fsp.Behaviors
{
    public class HttpClient
    {
        public static R MakePostRequest<T, R>(string url, T data)
        {
            Url site = new Url(url);
            Task<R> result = site.PostJsonAsync(data).ReceiveJson<R>();

            return result.Result;
        }
    }
}