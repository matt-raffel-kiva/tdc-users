using System;
using System.Collections.Generic;
using System.Text;

namespace fsp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string AppTitle => "FSP";
        public string TDCEndpoint => "http://localhost:3015"; 
        public TDCConnectionViewModel Connection => new TDCConnectionViewModel();
        public IssueOneTimeValueViewModel OneTimeValue => new IssueOneTimeValueViewModel();
    }
}