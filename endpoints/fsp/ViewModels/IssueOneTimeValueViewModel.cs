using ReactiveUI;
using fsp.Behaviors;
using fsp.Models;

namespace fsp.ViewModels
{
    public class IssueOneTimeValueViewModel : ViewModelBase
    {
        private string oneTimeValue = "not set";
        public string OneTimeValue
        {
            get => oneTimeValue; 
            set => this.RaiseAndSetIfChanged(ref oneTimeValue, value);
        }

        public void OnGenerateValue()
        {
            string url = $"http://localhost:3015/v2/fsp/register/onetimkey";
            string result =
                HttpClient.MakeGetRequestAsText(url);
            
            System.Diagnostics.Trace.WriteLine($"onetimevalue retrieved with '{result}'");
            OneTimeValue = result;
        }

        public void OnSendOneTimeValue()
        {
            string url = $"http://localhost:3013/v2/transaction/registerOnetimeKey";
            string result =
                HttpClient.MakePostRequest<IssueOneTimeKeyRequest, string>(url, new IssueOneTimeKeyRequest()
                    {
                        connectionId = "",
                        oneTimeKey = oneTimeValue,
                        tdcEndpoint = "http://tdc-controller:3015"
                    }
                );
            
            System.Diagnostics.Trace.WriteLine($"onetime key sent results: {result}");
        }
    }
}