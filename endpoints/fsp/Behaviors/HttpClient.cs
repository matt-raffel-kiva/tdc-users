using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace fsp.Behaviors
{
    public class HttpClient
    {
        static HttpClient()
        {
            FlurlHttp.Configure(settings =>
            {
                settings.OnErrorAsync = HttpClient.HandleFlurlErrorAsync;
                settings.BeforeCallAsync = HttpClient.HandleBeforeCallAsync;
                // please note: enabling AfterCallAsync handler causes callers to get null references since 
                // this method gets the result causing callers to get nothing.  
                // settings.AfterCallAsync = HttpClient.HandleAfterCallAsync;
            });    
        }
        
        private static async Task HandleAfterCallAsync(FlurlCall call)
        {
            System.Diagnostics.Trace.WriteLine($"body retrieved {call.Response.GetStringAsync().Result}");
        }
        
        private static async Task HandleBeforeCallAsync(FlurlCall call)
        {
            System.Diagnostics.Trace.WriteLine($"body sent {call.RequestBody}");
        }

        private static async Task HandleFlurlErrorAsync(FlurlCall call)
        {
            System.Diagnostics.Trace.WriteLine($"http exception: {call.Exception.Message}");
        }

        public static R MakePostRequest<T, R>(string url, T data)
        {
            Url site = new Url(url);
            Task<R> result = site.PostJsonAsync(data).ReceiveJson<R>();

            return result.Result;
        }
    }
}