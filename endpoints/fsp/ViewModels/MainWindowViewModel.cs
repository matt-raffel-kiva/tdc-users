using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Timers;
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
        private string tdcTroId = "not set";
        private string tdcFspId = "not set";
        private string status = string.Empty;
        private int progressBarValue = 0;
        private System.Timers.Timer progressTimer = null;
        #endregion
        
        #region ICommands
        private ReactiveCommand<Unit, Unit> OnConnectToTDC { get; }
        private ReactiveCommand<Unit, Unit> OnGenerateValue { get; }
        private ReactiveCommand<Unit, Unit> OnSendOneTimeValue { get; }
        private ReactiveCommand<Unit, Unit> OnGetReport { get; }
        private ReactiveCommand<Unit, Unit> OnCreateTransaction { get; }
        #endregion
        
        #region public observable data
        public string AppTitle => "FSP";
        
        public string Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }

        public int ProgressBarValue
        {
            get => progressBarValue;
            set => this.RaiseAndSetIfChanged(ref progressBarValue, value);
        }

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
            OnGetReport = ReactiveCommand.Create(GetReport);
            OnCreateTransaction = ReactiveCommand.Create(CreateTransaction);
        }
        
        public bool CanOnConnectTDC()
        {
            return true;    
        }
        #endregion

        #region private UI manipulation

        private void StartProgressBar()
        {
            Status = String.Empty;
            ProgressBarValue = 0;
            if (null == progressTimer)
            {
                progressTimer = new Timer();
                progressTimer.Interval = 250;
                progressTimer.Elapsed += (sender, args) =>
                {
                    ProgressBarValue += 1;
                    if (ProgressBarValue > 100)
                        ProgressBarValue = 0;
                };
            }

            progressTimer.Start();
        }

        private void StopProgressBar()
        {
            ProgressBarValue = 0;
            progressTimer.Stop();
        }

        private async void ExecuteLongRunningJob(string jobName, Action work)
        {
            await Task.Run(() =>
            {
                try
                {
                    DateTime start = DateTime.Now;
                    StartProgressBar();
                    work();

                    Status = $"{(DateTime.Now - start):ss}s";
                }
                catch (Exception ex)
                {
                    Status = ex.Message;
                    System.Diagnostics.Trace.WriteLine($"{jobName} error: {ex.Message}");
                    System.Diagnostics.Trace.WriteLineIf(ex.InnerException != null,
                        $"     inner exception {ex.InnerException.Message}");
                }
                finally
                {
                    StopProgressBar();
                }
            });
        }
        #endregion
        
        #region private methods
        private void ConnectTDC()
        {
            ExecuteLongRunningJob("ConnectTDC", () =>
            {
                string siteUrl = $"{url}/v2/transaction/register";
                TDCConnectionResult result =
                    HttpClient.MakePostRequest<TDCConnectionRequest, TDCConnectionResult>(siteUrl,
                        new TDCConnectionRequest()
                        {
                            tdcPrefix = "TDC",
                            tdcEndpoint = TDCDockerEndPoint
                        }
                    );

                ConnectionId = result.connectionData.connection_id;
            });
        }
        
        private void GenerateValue()
        {
            ExecuteLongRunningJob("GenerateValue", () =>
            {
                string siteUrl = $"{TDCLocalEndpoint}/v2/fsp/register/onetimkey";
                string result =
                    HttpClient.MakeGetRequestAsText(siteUrl);

                OneTimeValue = result;
            });
        }

        private void SendOneTimeValue()
        {
            ExecuteLongRunningJob("SendOneTimeValue", () =>
                {
                    string siteUrl = $"{this.url}/v2/transaction/registerOnetimeKey";
                    IssueOneTimeKeyResponse result =
                        HttpClient.MakePostRequest<IssueOneTimeKeyRequest, IssueOneTimeKeyResponse>(siteUrl,
                            new IssueOneTimeKeyRequest()
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
                });
        }

        private void GetReport()
        {
            ExecuteLongRunningJob("GetReport", () =>
            {
                string siteUrl = $"{this.url}/v2/transaction/report";

                string result =
                    HttpClient.MakePostRequestAsText<GetReportRequest>(siteUrl,
                        new GetReportRequest()
                        {
                            tdcFspId = tdcFspId,
                            tdcTroId = tdcTroId,
                            tdcEndpoint = TDCDockerEndPoint
                        }
                    );
            });
        }

        private void CreateTransaction()
        {
            ExecuteLongRunningJob("CreateTransaction", () =>
            {
                string siteUrl = $"{this.url}/v2/transaction/createTransaction";

                string result =
                    HttpClient.MakePostRequestAsText<GetReportRequest>(siteUrl,
                        new GetReportRequest()
                        {
                            tdcFspId = tdcFspId,
                            tdcTroId = tdcTroId,
                            tdcEndpoint = TDCDockerEndPoint
                        }
                    );
            });
        }
        #endregion
    }
}