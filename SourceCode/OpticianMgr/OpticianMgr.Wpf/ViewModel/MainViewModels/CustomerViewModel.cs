using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class CustomerViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenCustomer { get; set; }
        public ICommand AddCustomer { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ICollectionView CustomersView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public string TranslatedSortProperty { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public string SortProperty { get; set; }
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Customer).GetProperties().Select(c => c.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp"); //Shouldnt be able to filter by timestamp
                props.Remove("Town_Id"); //Shouldnt be able to filter by town_id
                props.Remove("Country_Id");
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        public CustomerViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.SortProperty = "Id";
            this.FilterProperty = "Nachname";
            this.Customers = GetAllCustomers();
            this.CustomersView = CollectionViewSource.GetDefaultView(Customers);
            OpenCustomer = new RelayCommand(OpenC);
            AddCustomer = new RelayCommand(AddC);
            FilterAndSort = new RelayCommand(FilterAndSortCustomers);
            DeleteFilter = new RelayCommand(DeleteF);

            EventHandler<EventArgs> refreshCustomers = null;
            refreshCustomers = (sender, e) =>
            {
                this.FillList();
            };
            ViewModelLocator.TownDetailsViewModel.Refresh += refreshCustomers;
            ViewModelLocator.CountryDetailsViewModel.Refresh += refreshCustomers;
        }
        public void DeleteF()
        {
            this.FilterText = "";
            FilterAndSortCustomers();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Customers = GetAllCustomers();
            this.RaisePropertyChanged(() => this.Customers);
            this.CustomersView = CollectionViewSource.GetDefaultView(Customers);
            FilterAndSortCustomers();
            this.RaisePropertyChanged(() => this.CustomersView);
        }
        public void FilterAndSortCustomers()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterProperty = this.FilterProperty == "Ort" ? "Ortsname" : this.FilterProperty;
            this.FilterProperty = this.FilterProperty == "Land" ? "Landname" : this.FilterProperty;//Program filters by the name of the country and not by by country-object
            this.SortProperty = this.SortProperty == "Ort" ? "Ortsname" : this.SortProperty;
            this.SortProperty = this.SortProperty == "Land" ? "Landname" : this.SortProperty;
            this.TranslatedFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            this.TranslatedSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortProperty).Key?.ToString();
            Filter();
            Sort();
        }
        public void Sort()
        {
            try
            {
                this.CustomersView.SortDescriptions.Clear();
                if (typeof(Customer).GetProperty(TranslatedSortProperty) != null)
                    this.CustomersView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
                else if (typeof(Country).GetProperty(TranslatedSortProperty) != null)
                    this.CustomersView.SortDescriptions.Add(new SortDescription("Country." + TranslatedSortProperty, ListSortDirection.Ascending));
                else if (typeof(Town).GetProperty(TranslatedSortProperty) != null)
                    this.CustomersView.SortDescriptions.Add(new SortDescription("Town." + TranslatedSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.CustomersView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.CustomersView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool Contains(object c)
        {
            Customer customer = c as Customer;
            if (typeof(Customer).GetProperty(TranslatedFilterProperty) != null)
            {
                return customer.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(customer, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else if (typeof(Country).GetProperty(TranslatedFilterProperty) != null) //Does the user filter by countryname?
            {
                return customer.Country?.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(customer.Country, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else if (typeof(Town).GetProperty(TranslatedFilterProperty) != null) //Does the user filter by countryname?
            {
                return customer.Town?.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(customer.Town, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        public void OpenC()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                CustomerDetailsViewModel viewModel = ViewModelLocator.CustomerDetailsViewModel;
                viewModel.InitCustomer(((Customer)this.Selected).Id);
                EventHandler<EventArgs> refreshCustomersHandler = null;
                refreshCustomersHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshCustomersHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshCustomersHandler;
                windowService.ShowCustomerDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst einen Kunden aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void AddC()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddCustomerViewModel viewModel = ViewModelLocator.AddCustomerViewModel;
            EventHandler<EventArgs> refreshCustomerHandler = null;
            refreshCustomerHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshCustomerHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshCustomerHandler;
            windowService.ShowAddCustomerWindow(viewModel);
        }
        private ObservableCollection<Customer> GetAllCustomers()
        {
            var unitofWorkCustomers = this.Uow.CustomerRepository.Get().ToList();
            ObservableCollection<Customer> copiedCustomers = new ObservableCollection<Customer>();
            foreach (var item in unitofWorkCustomers)
            {
                Customer customer = new Customer();
                GenericRepository<Customer>.CopyProperties(customer, item);
                if (item.Town_Id != null)
                {
                    Town town = new Town(); //Referenced town must be copied as well
                    GenericRepository<Town>.CopyProperties(town, this.Uow.TownRepository.GetById(item.Town_Id));
                    customer.Town = town;
                }
                if (item.Country_Id != null)
                {
                    Country country = new Country();
                    GenericRepository<Country>.CopyProperties(country, this.Uow.CountryRepository.GetById(item.Country_Id));
                    customer.Country = country;
                }
                copiedCustomers.Add(customer);
            }
            return copiedCustomers;
        }
    }
}
