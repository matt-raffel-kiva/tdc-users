
using System;
using System.Collections.Generic;
using System.Text;
using fsp.Behaviors;
using fsp.Models;
using ReactiveUI;

namespace fsp.ViewModels
{
    public class TDCConnectionViewModel : ViewModelBase
    {
        private string url = "http://localhost:3013";
        private string connectionId = "not set";
        
        public string Url
        {
            get => url; 
            set => this.RaiseAndSetIfChanged(ref url, value);
        }

        public string ConnectionId
        {
            get => connectionId;
            set => this.RaiseAndSetIfChanged(ref connectionId, value);
        }

        public bool CanOnConnectTDC()
        {
            return true;    
        }

        public void OnConnectTDC()
        {
            string registerUrl = $"{url}/v2/transaction/register";
            TDCConnectionResult result =
                HttpClient.MakePostRequest<TDCConnectionRequest, TDCConnectionResult>(registerUrl, new TDCConnectionRequest()
                    {
                        tdcPrefix = "TDC",
                        tdcEndpoint = "http://tdc-controller:3015"
                    }
                );
            
            System.Diagnostics.Trace.WriteLine($"connection retrieved with id {result.connectionData.connection_id}");
            ConnectionId = result.connectionData.connection_id;
        }
    }
}