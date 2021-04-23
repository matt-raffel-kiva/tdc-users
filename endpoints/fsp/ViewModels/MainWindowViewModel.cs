using System;
using System.Reactive;
using ReactiveUI;
using fsp.Behaviors;
using fsp.Models;
using Newtonsoft.Json;

namespace fsp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region private data
        private string url = "http://localhost:3013";
        private string connectionId = "not set";
        private string oneTimeValue = "not set";
        private string tdcTroId = "not set";
        private string tdcFspId = "not set";
        #endregion
        
        #region ICommands
        public ReactiveCommand<Unit, Unit> OnConnectToTDC { get; }
        public ReactiveCommand<Unit, Unit> OnGenerateValue { get; }
        public ReactiveCommand<Unit, Unit> OnSendOneTimeValue { get; }
        #endregion
        
        #region public observable data
        public string AppTitle => "FSP";            
        public string TDCLocalEndpoint => "http://localhost:3015";  // TODO: this should be configurable
        public string TDCDockerEndPoint => "http://tdc-controller:3015"; // TODO: this could be configurable
        
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
        
        public string OneTimeValue
        {
            get => oneTimeValue; 
            set => this.RaiseAndSetIfChanged(ref oneTimeValue, value);
        }

        public string TdcTroId
        {
            get => tdcTroId;
            set => this.RaiseAndSetIfChanged(ref tdcTroId, value);
        }
        
        public string TdcFspId
        {
            get => tdcFspId;
            set => this.RaiseAndSetIfChanged(ref tdcFspId, value);
        }
        #endregion

        #region public methods
        public MainWindowViewModel() : base()
        {
            OnConnectToTDC = ReactiveCommand.Create(ConnectTDC);
            OnGenerateValue = ReactiveCommand.Create(GenerateValue);
            OnSendOneTimeValue = ReactiveCommand.Create(SendOneTimeValue);
        }
        
        public bool CanOnConnectTDC()
        {
            return true;    
        }
        #endregion

        #region private methods
        private void ConnectTDC()
        {
            string siteUrl = $"{url}/v2/transaction/register";
            TDCConnectionResult result =
                HttpClient.MakePostRequest<TDCConnectionRequest, TDCConnectionResult>(siteUrl, new TDCConnectionRequest()
                    {
                        tdcPrefix = "TDC",
                        tdcEndpoint = TDCDockerEndPoint
                    }
                );
            
            ConnectionId = result.connectionData.connection_id;
        }
        
        private void GenerateValue()
        {
            string siteUrl = $"{TDCLocalEndpoint}/v2/fsp/register/onetimkey";
            string result =
                HttpClient.MakeGetRequestAsText(siteUrl);
            
            OneTimeValue = result;
        }

        private void SendOneTimeValue()
        {
            try
            {
                string siteUrl = $"{this.url}/v2/transaction/registerOnetimeKey";
                IssueOneTimeKeyResponse result =
                    HttpClient.MakePostRequest<IssueOneTimeKeyRequest, IssueOneTimeKeyResponse>(siteUrl, new IssueOneTimeKeyRequest()
                        {
                            connectionId = connectionId,
                            oneTimeKey = oneTimeValue,
                            tdcEndpoint = TDCDockerEndPoint
                        }
                    );

                if (!string.IsNullOrEmpty(result.tdcFspId))
                    TdcFspId = result.tdcFspId;
                if (!string.IsNullOrEmpty(result.tdcTroId))
                    TdcTroId = result.tdcTroId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"onetime key sent error: {ex.Message}");
                System.Diagnostics.Trace.WriteLineIf(ex.InnerException != null, $"     inner exception {ex.InnerException.Message}");
            }
        }
        #endregion
    }
}