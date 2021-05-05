﻿using System;
using System.Globalization;
using System.Reactive;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using ReactiveUI;
using fsp.Behaviors;
using fsp.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fsp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region private data

        private static string NOT_SET = "not set";
        private static string WAITING = "waiting";
        private static string ACCEPTED = "accepted";
        private string url = "http://localhost:3013";
        private string connectionId = NOT_SET;
        private string oneTimeValue = NOT_SET;
        private string tdcTroId = NOT_SET;
        private string tdcFspId = NOT_SET;
        private string txId = NOT_SET;
        private string txStatus = NOT_SET;
        private string status = string.Empty;
        private string eventData = string.Empty;
        private string eventType = "payment";
        private string reportId = NOT_SET;
        private int progressBarValue = 0;
        private System.Timers.Timer progressTimer = null;
        #endregion
        
        #region ICommands
        private ReactiveCommand<Unit, Unit> OnConnectToTDC { get; }
        private ReactiveCommand<Unit, Unit> OnGenerateValue { get; }
        private ReactiveCommand<Unit, Unit> OnSendOneTimeValue { get; }
        private ReactiveCommand<Unit, Unit> OnGetReport { get; }
        private ReactiveCommand<Unit, Unit> OnCreateTransaction { get; }
        
        private ReactiveCommand<Unit, Unit> OnRefreshIds { get; }
        private ReactiveCommand<Unit, Unit> OnRefreshTransaction { get; }
        private ReactiveCommand<Unit, Unit> OnRefreshReport { get; }
        #endregion
        
        #region public observable data
        public string AppTitle => "FSP";

        public string AppDesc => "This represents an entity who issues transactions on the behalf of individuals.  Examples include a bank or NGO.";
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

        public string TxId
        {
            get => txId;
            set => this.RaiseAndSetIfChanged(ref txId, value);
        }

        public string TxStatus
        {
            get => txStatus;
            set => this.RaiseAndSetIfChanged(ref txStatus, value);
        }
        
        public string EventData
        {
            get => eventData;
            set => this.RaiseAndSetIfChanged(ref eventData, value);
        }
        
        public string EventType
        {
            get => eventType;
            set => this.RaiseAndSetIfChanged(ref eventType, value);
        }

        [NotNull]
        public string ReportId
        {
            get => reportId;
            set => this.RaiseAndSetIfChanged(ref reportId, value);
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
            OnRefreshIds = ReactiveCommand.Create(RefreshIds);
            OnRefreshTransaction = ReactiveCommand.Create(RefreshTransaction);
            OnRefreshReport = ReactiveCommand.Create(RefreshReport);
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
                    HttpClient.MakeGetRequest(siteUrl);

                OneTimeValue = result;
            });
        }

        private void SendOneTimeValue()
        {
            ExecuteLongRunningJob("SendOneTimeValue", () =>
                {
                    TdcTroId = "waiting....";
                    TdcFspId = "waiting....";

                    string siteUrl = $"{url}/v2/transaction/nonce";
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
                ReportId = WAITING;
                
                StartGetReportResult result =
                    HttpClient.MakePostRequest<StartGetReportRequest, StartGetReportResult>(siteUrl,
                        new StartGetReportRequest()
                        {
                            fspTdcId = tdcFspId,
                            troTdcId = tdcTroId,
                            tdcEndpoint = TDCDockerEndPoint
                        }
                    );
                
                System.Diagnostics.Debug.WriteLine($"GetReport results {result.reportId}");
                ReportId = result.reportId;
            });
        }

        private void CreateTransaction()
        {
            ExecuteLongRunningJob("CreateTransaction", () =>
            {
                string siteUrl = $"{url}/v2/transaction/create";
                TxId = WAITING;
                TxStatus = WAITING;

                CreateTransactionResult result =
                    HttpClient.MakePostRequest<CreateTransactionRequest, CreateTransactionResult>(siteUrl,
                        new CreateTransactionRequest()
                        {
                            fspId = TdcFspId,
                            eventDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            eventJson = eventData,
                            eventType = eventType,
                            tdcEndpoint = TDCDockerEndPoint,
                            fspHash = ComputeHash(eventData)
                        }
                    );
                
                System.Diagnostics.Debug.WriteLine($"transaction created {result.transactionId}");
                TxId = result.transactionId;
            });
        }

        private void RefreshIds()
        {
            ExecuteLongRunningJob("RefreshIds", () =>
            {
                string siteUrl = $"{url}/v2/transaction/ids/{oneTimeValue}";
                RefreshIdResult result =
                    HttpClient.MakeGetRequest<RefreshIdResult>(siteUrl);
                
                if (!string.IsNullOrEmpty(result.fsp_id))
                    TdcFspId = result.fsp_id;
                if (!string.IsNullOrEmpty(result.tdc_id))
                    TdcTroId = result.tdc_id;
            });
        }

        private void RefreshTransaction()
        {
            ExecuteLongRunningJob("RefreshTransaction", () =>
            {
                string siteUrl = $"{url}/v2/transaction/{txId}";
                RefreshTransactionResult result =
                    HttpClient.MakeGetRequest<RefreshTransactionResult>(siteUrl);

                // TODO: like to make this more detailed.  for now, if we get a transaction back
                // then we can assume its accepted
                TxStatus = ACCEPTED;
            });            
        }

        private void RefreshReport()
        {
            ExecuteLongRunningJob("RefreshTransaction", () =>
            {
                string siteUrl = $"{url}/v2/transaction/{txId}";
                RefreshTransactionResult result =
                    HttpClient.MakeGetRequest<RefreshTransactionResult>(siteUrl);

                // TODO: like to make this more detailed.  for now, if we get a transaction back
                // then we can assume its accepted
                TxStatus = ACCEPTED;
            });       
        }
        private string ComputeHash(string input)
        {
            //
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=netframework-4.8
            //
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                var hashBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.

                for (int i = 0; i < data.Length; i++)
                {
                    hashBuilder.Append(data[i].ToString("x2"));
                }
                
                return hashBuilder.ToString();
            }
        }
        #endregion
    }
}