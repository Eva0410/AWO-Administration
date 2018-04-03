using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using OpticiatnMgr.Core.Contracts;
using System.Globalization;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using OpticiatnMgr.Core.Entities;

namespace OpticianMgr.Wpf.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        public int OldYear { get; set; }
        public int NewYear { get; set; }
        public ObservableCollection<KeyValuePair<string, int>> NewValues { get; set; }
        public ObservableCollection<KeyValuePair<string, int>> OldValues { get; set; }
        private IUnitOfWork Uow { get; set; }
        public ICommand ChangeStatistic { get; set; }
        public string Title { get; set; }
        public StatisticsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            NewYear = DateTime.Now.Year;
            OldYear = NewYear - 1;
            this.NewValues = new ObservableCollection<KeyValuePair<string, int>>();
            this.OldValues = new ObservableCollection<KeyValuePair<string, int>>();
            this.ChangeStatistic = new RelayCommand<string>(ChangeStats);

            this.ChangeStats("B");

            this.RaisePropertyChanged(() => this.NewYear);
            this.RaisePropertyChanged(() => this.OldYear);
        }
        private void ChangeStats(string orderType)
        {
            this.OldValues.Clear();
            this.NewValues.Clear();
            this.FillList(this.NewValues, NewYear, orderType);
            this.FillList(this.OldValues, OldYear, orderType);
            if (orderType == "B")
            {
                this.Title = "Verkaufte Brillen";
            }
            else
            {
                this.Title = "Verkaufte Kontaktlinsen";
            }
            this.RaisePropertyChanged(() => this.NewValues);
            this.RaisePropertyChanged(() => this.OldValues);
            this.RaisePropertyChanged(() => this.Title);
        }
        private void FillList(ObservableCollection<KeyValuePair<string, int>> list, int year, string orderType)
        {
            for (int i = 1; i <= 12; i++)
            {
                int soldItems = this.Uow.OrderRepository.Count(o => o.PaymentDate != null && o.PaymentDate.Value.Year == year && o.PaymentDate.Value.Month == i && o.PaymentState == "Bezahlt" && o.OrderType == orderType);
                list.Add(new KeyValuePair<string, int>(new DateTime(2017, i, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("de")), soldItems));
            }
        }
        private bool Check(object o, int year, int i, string orderType)
        {
            bool check = false;
            var order = (Order) o;
            if(order.PaymentDate != null)
            {
                if(order.PaymentDate.Value.Year == year && order.PaymentDate.Value.Month == i && order.PaymentState == "Bezahlt" && order.OrderType == orderType)
                {
                    check = true;
                }
            }
            return check;
        }
    }
}
