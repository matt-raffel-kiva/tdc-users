using System;
using System.Collections.Generic;
using System.Text;

namespace fsp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string AppTitle => "FSP";
        public TDCConnectionViewModel Connection => new TDCConnectionViewModel();
    }
}