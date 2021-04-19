
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
        private string url = "http://localhost:3015";
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
            TDCConnectionResult result =
                HttpClient.MakePostRequest<TDCConnectionRequest, TDCConnectionResult>(url, new TDCConnectionRequest()
                    {
                        TdcPrefix = "TDC",
                        TdcUrl = this.url
                    }
                );
        }
    }
}