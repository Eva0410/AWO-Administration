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
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand SortShift { get; set; }
        public ICommand Initialized { get; set; }

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
        public List<GridViewColumnHeader> SortHeaders { get; set; }
        public List<GridViewColumnHeader> AllHeaders { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.FilterProperty = "Name";
            this.Suppliers = GetAllSuppliers();
            this.SuppliersView = CollectionViewSource.GetDefaultView(Suppliers);
            FilterAndSort = new RelayCommand(FilterSuppliers);
            DeleteFilter = new RelayCommand(DeleteF);
            AddSupplier = new RelayCommand(AddS);
            OpenSupplier = new RelayCommand(OpenS);
            SortCommand = new RelayCommand<RoutedEventArgs>(SortS);
            SortShift = new RelayCommand<object>(SortSh);
            SortHeaders = new List<GridViewColumnHeader>();
            Initialized = new RelayCommand<RoutedEventArgs>(Init);

            EventHandler<EventArgs> refreshSuppliers = null;
            refreshSuppliers = (sender, e) =>
            {
                this.FillList();
            };
            ViewModelLocator.TownDetailsViewModel.Refresh += refreshSuppliers;
            ViewModelLocator.CountryDetailsViewModel.Refresh += refreshSuppliers;
        }
        private void Init(RoutedEventArgs p )
        {
            AllHeaders = new List<GridViewColumnHeader>();
            ListView lv = p.Source as ListView;
            GridView gv = (GridView)lv.View;
            foreach (var item in gv.Columns)
            {
                AllHeaders.Add(item.Header as GridViewColumnHeader);
            }
        }
        //Click without shift key
        private void SortS(RoutedEventArgs e)
        {
            GridViewColumnHeader columnHeader = e.Source as GridViewColumnHeader;

            if (columnHeader == null)
            {
                return;
            }

            ListSortDirection dir;
            string header;
            //Same column pressed?
            if (SortHeaders.Count > 0 && SortHeaders[0] == columnHeader)
            {
                //Change sort direction
                dir = this.SuppliersView.SortDescriptions[0].Direction;
                dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                header = ChangeArrow(columnHeader, dir);
            }
            else
            {
                //Remove arrow from old column header
                if (SortHeaders.Count > 0)
                {
                    foreach (var item in SortHeaders)
                    {
                        item.Column.HeaderTemplate = null;
                        item.Column.Width = item.ActualWidth - 20;
                    }
                }
                SortHeaders.Clear();
                SortHeaders.Add(columnHeader);
                //default sort direction is ascending
                dir = ListSortDirection.Ascending;
                header = SetNewArrow(columnHeader, dir);
            }
            this.SuppliersView.SortDescriptions.Clear();
            this.SuppliersView.SortDescriptions.Add(new SortDescription(header, dir));
        }
        private string SetNewArrow(GridViewColumnHeader column, ListSortDirection dir)
        {
            //new column header must be wider
            column.Column.Width = column.ActualWidth + 20;

            return ChangeArrow(column, dir);
        }
        private string ChangeArrow(GridViewColumnHeader column,ListSortDirection dir)
        {

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
            Binding b = column.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }
            return header;
        }
        //Click with shift
        private void SortSh(object p)
        {
            string columnName = p as string;
            string germanColumnName = TranslateEnglishToGerman(columnName);
            //Get gridviewcolumnheader
            var columnHeader = AllHeaders.Where(h => String.Equals(h.Content.ToString(), germanColumnName)).ToList().FirstOrDefault();

            if(columnHeader == null)
            {
                return;
            }
            if (this.SuppliersView.SortDescriptions.Count >= 1)
            {
                ListSortDirection dir;
                int index = this.SuppliersView.SortDescriptions.Count - 1;
                //Change sorting direction
                if (this.SuppliersView.SortDescriptions.Count == index + 1 && this.SuppliersView.SortDescriptions[index].PropertyName == columnName)
                {
                    dir = this.SuppliersView.SortDescriptions[index].Direction;
                    dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.SuppliersView.SortDescriptions.RemoveAt(index);
                    this.SuppliersView.SortDescriptions.Insert(index, new SortDescription(columnName, dir));
                    ChangeArrow(columnHeader, dir);
                    SortHeaders.Add(columnHeader);
                }
                else if(this.SuppliersView.SortDescriptions.Count(s => s.PropertyName == columnName) == 0)
                {
                    if (this.SuppliersView.SortDescriptions.Count >= 3)
                    {
                        MessageBox.Show("Sie können maximal nach drei Spalten sortieren!", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    dir = ListSortDirection.Ascending;
                    SetNewArrow(columnHeader, dir);
                    this.SuppliersView.SortDescriptions.Add(new SortDescription(columnName, dir));
                    SortHeaders.Add(columnHeader);
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
        private string TranslateEnglishToGerman(string englishName)
        {
            if (englishName.Contains("."))
            {
                englishName = englishName.Split('.')[1];
            }
            string german = manager.GetString(englishName);
            german = german == "Ortsname" ? "Ort" : german;
            german = german == "Landname" ? "Land" : german;
            return german;
        }
        private string TranslateGermanToEnglish(string germanName)
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            germanName = germanName == "Ort" ? "Ortsname" : germanName; //Program filters by the name of the town and not by by town-object
            germanName = germanName == "Land" ? "Landname" : germanName;
            return dictionary.FirstOrDefault(e => e.Value.ToString() == germanName).Key?.ToString();
        }
        /// <summary>
        /// Refreshes and filter the supplier list
        /// </summary>
        public void FilterSuppliers()
        {
            this.TranslatedFilterProperty = TranslateGermanToEnglish(this.FilterProperty);
            Filter();
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
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
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
            this.FilterSuppliers();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Suppliers = GetAllSuppliers();
            this.RaisePropertyChanged(() => this.Suppliers);
            var sort = this.SuppliersView.SortDescriptions;
            this.SuppliersView = CollectionViewSource.GetDefaultView(Suppliers);
            this.SuppliersView.SortDescriptions.Clear();
            foreach (var item in sort)
            {
                this.SuppliersView.SortDescriptions.Add(item);
            }
            FilterSuppliers();
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
