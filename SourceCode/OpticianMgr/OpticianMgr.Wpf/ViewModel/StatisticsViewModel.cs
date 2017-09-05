using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.Controls;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace OpticianMgr.Wpf.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        public int OldYear { get; set; }
        public int NewYear { get; set; }
        public ObservableCollection<KeyValuePair<DateTime,int>> Values { get; set; }
        public StatisticsViewModel()
        {
            NewYear = DateTime.Now.Year;
            OldYear = NewYear - 1;
            this.Values = new ObservableCollection<KeyValuePair<DateTime, int>>{
        new KeyValuePair<DateTime,int>(DateTime.Now, 100),
        new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(1), 130),
        new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(2), 150),
        new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(3), 125),
        new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(4),155) };
            this.RaisePropertyChanged(() => this.NewYear);
            this.RaisePropertyChanged(() => this.OldYear);
            this.RaisePropertyChanged(() => this.Values);
        }
        //Bindings im LineSeries funktionieren nicht, anderes Tool?
    }
}
