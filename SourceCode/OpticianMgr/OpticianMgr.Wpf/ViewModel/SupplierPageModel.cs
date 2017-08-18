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
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows;

namespace OpticianMgr.Wpf.ViewModel
{ 
    public class SupplierPageModel : ViewModelBase
    {
        private IUnitOfWork uow;
        private ObservableCollection<Lieferant> supplierList;

        private Lieferant _selectedLieferant;

        public Lieferant SelectedLieferant
        {
            get { return _selectedLieferant; }
            set { _selectedLieferant = value; }
        }
        private DataGridCellInfo _selectedCell;

        public DataGridCellInfo SelectedCell
        {
            get { return _selectedCell; }
            set { _selectedCell = value; }
        }



        public ObservableCollection<Lieferant> SupplierList
        {
            get
            {
                return supplierList;
            }
            set
            {
                supplierList = value;
            }
        }
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                var li = new ObservableCollection<string>(typeof(Lieferant).GetProperties().Select(p => p.Name).ToList());
                li.Remove("Timestamp");
                li[li.IndexOf("Ort_Id")] = "PLZ";
                return li;
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
        public ICommand AddSupplier { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierPageModel()
        {
            this.uow = new UnitOfWork();
            this.SupplierList = GetAllSuppliers();
            FilterSuppliers = new RelayCommand(Filter);
            DeleteFilter = new RelayCommand(DeleteF);
            AddSupplier = new RelayCommand(AddS);
            this.SupplierList.CollectionChanged += this.OnCollectionChanged;

        }
        //TODO Immer die selbe collection verwenden und nicht jedes Mal neu erstellen
        private ObservableCollection<Lieferant> GetAllSuppliers()
        {
            ObservableCollection<Lieferant> collection = new ObservableCollection<Lieferant>(this.uow.LieferantenRepository.Get(includeProperties:"Ort").ToList());
            collection.CollectionChanged += this.OnCollectionChanged;
            return collection;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Delete Items
            if (e.OldItems != null)
            {
                foreach (Lieferant item in e.OldItems)
                {
                    if (item.Id != 0 && uow.LieferantenRepository.GetById(item.Id) != null)
                    {
                        var messageBoxResult = MessageBox.Show("Wollen Sie den Lieferanten '" + item.Lieferantenname + "' wirklich löschen?", "Lieferant Löschen", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            this.uow.LieferantenRepository.Delete(item);
                        }
                    }
                }
                this.uow.Save();
            }

            //Initialize new Items
            if(e.NewItems != null)
            {
                foreach (Lieferant item in e.NewItems)
                {
                    item.Ort = new Ort();
                }
            }
            this.SupplierList = GetAllSuppliers();
            this.RaisePropertyChanged(() => this.SupplierList);
        }

        public void Filter()
        {
            this.FilterProperty = this.FilterProperty == "Ort" ? "OrtName" : this.FilterProperty;
            if (this.filterText != "")
            {
                if (typeof(Ort).GetProperty(this.FilterProperty) != null)
                {
                    this.SupplierList = new ObservableCollection<Lieferant>(GetAllSuppliers().Where(s => s.Ort.GetType().GetProperty(this.filterProperty).GetValue(s.Ort, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));

                }
                else
                {
                    this.SupplierList = new ObservableCollection<Lieferant>(GetAllSuppliers().Where(s => s.GetType().GetProperty(this.filterProperty).GetValue(s, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));
                }
                this.SupplierList.CollectionChanged += OnCollectionChanged;
            }
            else
                this.SupplierList = GetAllSuppliers();
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        public void DeleteF()
        {
            this.SupplierList = GetAllSuppliers();
            this.FilterText = "";
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        public void AddS()
        {
            AddSupplierWindowService windowService = new AddSupplierWindowService();
            AddSupplierViewModel viewModel = new AddSupplierViewModel();
            windowService.showWindow(viewModel);
            EventHandler<EventArgs> handler = null;
            handler = (sender, e) =>
            {
                viewModel.RefreshSuppliers -= handler;
                this.SupplierList = this.GetAllSuppliers();
                this.RaisePropertyChanged(() => this.SupplierList);
            };
            viewModel.RefreshSuppliers += handler;
        }
    }
}
