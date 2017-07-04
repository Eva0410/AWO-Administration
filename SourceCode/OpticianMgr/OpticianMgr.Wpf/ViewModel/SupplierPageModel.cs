using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{ 
    public class SupplierPageModel : ViewModelBase
    {
        private List<Lieferant> fullSuppliers { get; set; }
        private IUnitOfWork uow;
        public ObservableCollection<Lieferant> supplierList;


        public ObservableCollection<Lieferant> SupplierList
        {
            get
            {
                return supplierList;
            }
            set { supplierList = value; }
        }
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                return new ObservableCollection<string>(typeof(Lieferant).GetProperties().Select(p => p.Name).ToList());
            }
        }
        private string filterProperty;

        public string FilterProperty
        {
            get { return filterProperty; }
            set { filterProperty = value; }
        }
        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                this.RaisePropertyChanged(() => this.FilterText);
            }
        }


        public ICommand FilterSuppliers { get; set; }
        public ICommand DeleteFilter { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierPageModel()
        {
            this.uow = new UnitOfWork();
            this.fullSuppliers = this.uow.LieferantenRepository.Get().ToList();
            this.supplierList = new ObservableCollection<Lieferant>(this.fullSuppliers);
            FilterSuppliers = new RelayCommand(Filter);
            DeleteFilter = new RelayCommand(DeleteF);
        }
        public void Filter()
        {
            this.SupplierList = new ObservableCollection<Lieferant>(this.fullSuppliers.Where(s => s.GetType().GetProperty(this.filterProperty).GetValue(s, null).ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0).ToList());
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        public void DeleteF()
        {
            this.supplierList = new ObservableCollection<Lieferant>(this.fullSuppliers);
            this.FilterText = "";
            this.RaisePropertyChanged(() => this.SupplierList);
        }
    }
}
