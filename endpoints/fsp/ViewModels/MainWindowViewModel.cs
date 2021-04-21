using System.Reactive;
using ReactiveUI;
using fsp.Behaviors;
using fsp.Models;

namespace fsp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region private data
        private string url = "http://localhost:3013";
        private string connectionId = "not set";
        private string oneTimeValue = "not set";
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
            string registerUrl = $"{url}/v2/transaction/register";
            TDCConnectionResult result =
                HttpClient.MakePostRequest<TDCConnectionRequest, TDCConnectionResult>(registerUrl, new TDCConnectionRequest()
                    {
                        tdcPrefix = "TDC",
                        tdcEndpoint = TDCDockerEndPoint
                    }
                );
            
            System.Diagnostics.Trace.WriteLine($"connection retrieved with id {result.connectionData.connection_id}");
            ConnectionId = result.connectionData.connection_id;
        }
        
        private void GenerateValue()
        {
            string url = $"{TDCLocalEndpoint}/v2/fsp/register/onetimkey";
            string result =
                HttpClient.MakeGetRequestAsText(url);
            
            System.Diagnostics.Trace.WriteLine($"onetimevalue retrieved with '{result}'");
            OneTimeValue = result;
        }

        private void SendOneTimeValue()
        {
            string url = $"{this.url}/v2/transaction/registerOnetimeKey";
            string result =
                HttpClient.MakePostRequest<IssueOneTimeKeyRequest, string>(url, new IssueOneTimeKeyRequest()
                    {
                        connectionId = connectionId,
                        oneTimeKey = oneTimeValue,
                        tdcEndpoint = TDCDockerEndPoint
                    }
                );
            
            System.Diagnostics.Trace.WriteLine($"onetime key sent results: {result}");
        }
        #endregion
    }
}