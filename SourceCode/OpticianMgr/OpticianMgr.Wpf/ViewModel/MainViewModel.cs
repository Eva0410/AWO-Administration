using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.Pages;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IUnitOfWork uow;
        private CustomerPage CustomerPage { get; set; }
        private StatisticsPage StatisticsPage { get; set; }
        private SupplierPage SupplierPage { get; set; }
        private EyeGlassFramesPage EyeGlassFramesPage { get; set; }
        private OrdersPage OrdersPage { get; set; }
        private MultipleMessagesPage MultipleMessagesPage { get; set; }

        public ICommand Suppliers { get; set; }
        public ICommand Customers { get; set; }
        public ICommand Statistics { get; set; }
        public ICommand EyeGlassFrames { get; set; }
        public ICommand Orders { get; set; }
        public ICommand MultipleMessages { get; set; }
        public object Page { get; set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.uow = new UnitOfWork();
            this.CustomerPage = new CustomerPage();
            this.SupplierPage = new SupplierPage();
            this.StatisticsPage = new StatisticsPage();
            this.EyeGlassFramesPage = new EyeGlassFramesPage();
            this.OrdersPage = new OrdersPage();
            this.MultipleMessagesPage = new MultipleMessagesPage();
            Suppliers = new RelayCommand(() => this.Open(this.SupplierPage));
            Customers = new RelayCommand(() => this.Open(this.CustomerPage));
            Statistics = new RelayCommand(() => this.Open(this.StatisticsPage));
            EyeGlassFrames = new RelayCommand(() => this.Open(this.EyeGlassFramesPage));
            Orders = new RelayCommand(() => this.Open(this.OrdersPage));
            MultipleMessages = new RelayCommand(() => this.Open(this.MultipleMessagesPage));
            this.Open(this.CustomerPage);
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

        }
        //TODO Is this MVVM?
        private void Open(object page)
        {
            this.Page = page;
            this.RaisePropertyChanged(() => this.Page);
        }
    }
}