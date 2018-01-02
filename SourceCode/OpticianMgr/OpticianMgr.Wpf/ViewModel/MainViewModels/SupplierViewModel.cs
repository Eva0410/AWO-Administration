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
using System.Resources;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;

namespace OpticianMgr.Wpf.ViewModel
{
    public class SupplierViewModel : ViewModelBase
    {
        public IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }
        public ObservableCollection<Supplier> Suppliers { get; set; }
        public ICollectionView SuppliersView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ICommand AddSupplier { get; set; }
        public ICommand OpenSupplier { get; set; }
        public string TranslatedSortProperty { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public string SortProperty { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand SortShift { get; set; }

        //The properties of the supplier class are safed in English but need to be shown in German
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Supplier).GetProperties().Select(p => p.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp"); //Shouldnt be able to filter by timestamp
                props.Remove("Country_Id"); //Shouldnt be able to filter by country_Id
                props[props.IndexOf("Town_Id")] = "ZipCode"; //shouldnt be able to filter by town_id but user should be able to filter by zipcode
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        public List<GridViewColumnHeader> Headers { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.FilterProperty = "Name";
            this.SortProperty = "Id";
            this.Suppliers = GetAllSuppliers();
            this.SuppliersView = CollectionViewSource.GetDefaultView(Suppliers);
            FilterAndSort = new RelayCommand(FilterAndSortSuppliers);
            DeleteFilter = new RelayCommand(DeleteF);
            AddSupplier = new RelayCommand(AddS);
            OpenSupplier = new RelayCommand(OpenS);
            SortCommand = new RelayCommand<RoutedEventArgs>(SortS);
            SortShift = new RelayCommand<object>(SortSh);
            Headers = new List<GridViewColumnHeader>();

            EventHandler<EventArgs> refreshSuppliers = null;
            refreshSuppliers = (sender, e) =>
            {
                this.FillList();
            };
            ViewModelLocator.TownDetailsViewModel.Refresh += refreshSuppliers;
            ViewModelLocator.CountryDetailsViewModel.Refresh += refreshSuppliers;
        }
        //Click without shift key
        private void SortS(RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.Source as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            ListSortDirection dir;
            //Same column pressed?
            if (Headers.Count > 0 && Headers[0] == column)
            {
                //Change sort direction
                dir = this.SuppliersView.SortDescriptions[0].Direction;
                dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                //Remove arrow from old column header
                if (Headers.Count > 0)
                {
                    Headers[0].Column.HeaderTemplate = null;
                    Headers[0].Column.Width = Headers[0].ActualWidth - 20;
                }
                Headers.Clear();
                Headers.Add(column);
                //new column header must be wider
                column.Column.Width = column.ActualWidth + 20;
                //default sort direction is ascending
                dir = ListSortDirection.Ascending;
            }
            //insert arrow
            if (dir == ListSortDirection.Ascending)
            {
                column.Column.HeaderTemplate = Application.Current.FindResource("ArrowUp") as DataTemplate;
            }
            else
            {
                column.Column.HeaderTemplate = Application.Current.FindResource("ArrowDown") as DataTemplate;
            }

            string header = string.Empty;

            // get binding name for sort description 
            Binding b = Headers[0].Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }

            this.SuppliersView.SortDescriptions.Clear();
            this.SuppliersView.SortDescriptions.Add(new SortDescription(header, dir));
        }
        //Click with shift
        private void SortSh(object p)
        {
            string column = p as string;
            if(this.SuppliersView.SortDescriptions.Count >= 1)
            {
                ListSortDirection dir;
                int index = this.SuppliersView.SortDescriptions.Count - 1;
                //Change sorting direction
                if (this.SuppliersView.SortDescriptions.Count == index + 1 && this.SuppliersView.SortDescriptions[index].PropertyName == column)
                {
                    dir = this.SuppliersView.SortDescriptions[index].Direction;
                    dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.SuppliersView.SortDescriptions.RemoveAt(index);
                    this.SuppliersView.SortDescriptions.Insert(index, new SortDescription(column, dir));
                }
                else
                {
                    dir = ListSortDirection.Ascending;
                    this.SuppliersView.SortDescriptions.Add(new SortDescription(column, dir));
                }
            }
        }
        /// <summary>
        /// Returns a list of the suppliers in the database
        /// All properties must be copied, otherwise the list would reference the unit of work data
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Supplier> GetAllSuppliers()
        {
            var unitOfWorkSuppliers = this.Uow.SupplierRepository.Get().ToList();
            ObservableCollection<Supplier> copiedSuppliers = new ObservableCollection<Supplier>();
            foreach (var item in unitOfWorkSuppliers)
            {
                Supplier sup = new Supplier();
                GenericRepository<Supplier>.CopyProperties(sup, item);
                if (item.Town_Id != null)
                {
                    Town town = new Town(); //Referenced town must be copied as well
                    GenericRepository<Town>.CopyProperties(town, this.Uow.TownRepository.GetById(item.Town_Id));
                    sup.Town = town;
                }
                if (item.Country_Id != null)
                {
                    Country country = new Country();
                    GenericRepository<Country>.CopyProperties(country, this.Uow.CountryRepository.GetById(item.Country_Id));
                    sup.Country = country;
                }
                copiedSuppliers.Add(sup);
            }
            return copiedSuppliers;
        }
        /// <summary>
        /// Refreshes and filter the supplier list
        /// </summary>
        public void FilterAndSortSuppliers()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterProperty = this.FilterProperty == "Ort" ? "Ortsname" : this.FilterProperty; //Program filters by the name of the town and not by by town-object
            this.FilterProperty = this.FilterProperty == "Land" ? "Landname" : this.FilterProperty;
            this.SortProperty = this.SortProperty == "Ort" ? "Ortsname" : this.SortProperty;
            this.SortProperty = this.SortProperty == "Land" ? "Landname" : this.SortProperty;
            this.TranslatedFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            this.TranslatedSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortProperty).Key?.ToString();
            Filter();
            //Sort();
        }
        public void Sort()
        {
            try
            {
                this.SuppliersView.SortDescriptions.Clear();
                if (typeof(Supplier).GetProperty(TranslatedSortProperty) != null)
                    this.SuppliersView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
                else if (typeof(Country).GetProperty(TranslatedSortProperty) != null)
                    this.SuppliersView.SortDescriptions.Add(new SortDescription("Country." + TranslatedSortProperty, ListSortDirection.Ascending));
                else if (typeof(Town).GetProperty(TranslatedSortProperty) != null)
                    this.SuppliersView.SortDescriptions.Add(new SortDescription("Town." + TranslatedSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.SuppliersView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.SuppliersView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool Contains(object s)
        {
            Supplier supplier = s as Supplier;
            if (typeof(Supplier).GetProperty(TranslatedFilterProperty) != null)
            {
                return supplier.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(supplier, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else if (typeof(Country).GetProperty(TranslatedFilterProperty) != null) //Does the user filter by countryname?
            {
                return supplier.Country?.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(supplier.Country, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else if (typeof(Town).GetProperty(TranslatedFilterProperty) != null) //Does the user filter by countryname?
            {
                return supplier.Town?.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(supplier.Town, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        //Delete filter
        public void DeleteF()
        {
            this.FilterText = "";
            this.FilterAndSortSuppliers();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Suppliers = GetAllSuppliers();
            this.RaisePropertyChanged(() => this.Suppliers);
            this.SuppliersView = CollectionViewSource.GetDefaultView(Suppliers);
            FilterAndSortSuppliers();
            this.RaisePropertyChanged(() => this.SuppliersView);
        }
        //add supplier
        public void AddS()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddSupplierViewModel viewModel = ViewModelLocator.AddSupplierViewModel;
            EventHandler<EventArgs> refreshSupplierHandler = null;
            refreshSupplierHandler = (sender, e) =>
            {
                viewModel.RefreshSuppliers -= refreshSupplierHandler;
                this.FillList();
            };
            viewModel.RefreshSuppliers += refreshSupplierHandler;
            windowService.ShowAddSupplierWindow(viewModel);
        }
        //open supplier 
        public void OpenS()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                SupplierDetailsViewModel viewModel = ViewModelLocator.SupplierDetailsViewModel;
                viewModel.InitSupplier(((Supplier)this.Selected).Id);
                EventHandler<EventArgs> refreshSupplierHandler = null;
                refreshSupplierHandler = (sender, e) =>
                {
                    viewModel.RefreshSuppliers -= refreshSupplierHandler;
                    this.FillList();
                };
                viewModel.RefreshSuppliers += refreshSupplierHandler;
                windowService.ShowSupplierDetailsWindow(viewModel);
            }
        }
    }
}
