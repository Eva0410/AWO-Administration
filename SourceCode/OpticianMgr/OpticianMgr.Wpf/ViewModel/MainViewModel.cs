using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
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
        
        public ICommand Suppliers { get; set; }

        public ICommand Customers { get; set; }
        public object Page { get; set; }

        //TODO
        //public ObservableCollection<TestEntity> Testlist
        //{
        //    get {
        //        return new ObservableCollection<TestEntity>(this.uow.TestRepository.Get());
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.uow = new UnitOfWork();
            Suppliers = new RelayCommand(OpenSuppliers);
            Customers = new RelayCommand(OpenCustomers);
            this.OpenSuppliers();
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
        private void OpenSuppliers()
        {
            SupplierPage newPage = new SupplierPage();
            this.Page = newPage;
            this.RaisePropertyChanged(() => this.Page);
        }
        private void OpenCustomers()
        {
            CustomerPage newPage = new CustomerPage();
            this.Page = newPage;
            this.RaisePropertyChanged(() => this.Page);
        }
    }
}