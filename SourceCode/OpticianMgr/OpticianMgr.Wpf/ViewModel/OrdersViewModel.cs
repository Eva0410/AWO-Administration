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
    public class OrdersViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public object SelectedGlasses { get; set; }
        public ICommand OpenGlasses { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<Order> Glasses { get; set; }
        public ICollectionView GlassesView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public string TranslatedGlassesSortProperty { get; set; }
        public string FilterGlassesProperty { get; set; }
        public string TranslatedGlassesFilterProperty { get; set; }
        public string FilterGlassesText { get; set; }
        public string SortGlassesProperty { get; set; }
        public ObservableCollection<String> GlassesPropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Order ).GetProperties().Select(c => c.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                //shouldnt be able to filter by these properties
                props.Remove("Timestamp");
                props.Remove("Customer_Id");
                props.Remove("Doctor_Id");
                props.Remove("Bill");
                props.Remove("OrderType");
                props.Remove("ContactLensType_Id");
                props.Remove("ContactLensType");
                props.Remove("ContactLensOthers1");
                props.Remove("ContactLensOthers2");
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        public OrdersViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.SortGlassesProperty = "Id";
            this.FilterGlassesProperty = "Nachname";
            this.Glasses = GetAllCustomers();
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            OpenGlasses = new RelayCommand(OpenC);
            FilterAndSort = new RelayCommand(FilterAndSortGlasses);
            DeleteFilter = new RelayCommand(DeleteGlassesF);
        }
        public void DeleteGlassesF()
        {
            this.FilterGlassesText = "";
            FilterAndSortGlasses();
            this.RaisePropertyChanged(() => this.FilterGlassesText);
        }
        public void FillGlassesList()
        {
            this.Glasses = GetAllCustomers();
            this.RaisePropertyChanged(() => this.Glasses);
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            FilterAndSortGlasses();
            this.RaisePropertyChanged(() => this.GlassesView);
        }
        public void FilterAndSortGlasses()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Doktor" ? "Doktorname" : this.FilterGlassesProperty;
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Kunde" ? "Nachname" : this.FilterGlassesProperty;//Program filters by the name of the country and not by by country-object
            this.SortGlassesProperty = this.SortGlassesProperty == "Doktor" ? "Doktorname" : this.SortGlassesProperty;
            this.SortGlassesProperty = this.SortGlassesProperty == "Kunde" ? "Nachname" : this.SortGlassesProperty;
            this.TranslatedGlassesFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterGlassesProperty).Key?.ToString();
            this.TranslatedGlassesSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortGlassesProperty).Key?.ToString();
            Filter();
            SortGlasses();
        }
        public void SortGlasses()
        {
            try
            {
                this.GlassesView.SortDescriptions.Clear();
                if (typeof(Customer).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription(this.TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Doctor).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("Country." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Town).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("Town." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterGlassesText))
                {
                    this.GlassesView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.GlassesView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool Contains(object c)
        {
            Customer customer = c as Customer;
            if (typeof(Customer).GetProperty(TranslatedGlassesFilterProperty) != null)
            {
                return customer.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(customer, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(Country).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return customer.Country?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(customer.Country, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(Town).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return customer.Town?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(customer.Town, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        public void OpenC()
        {
            if (this.SelectedGlasses != null)
            {
                WindowService windowService = new WindowService();
                CustomerDetailsViewModel viewModel = ViewModelLocator.CustomerDetailsViewModel;
                viewModel.InitCustomer(((Customer)this.SelectedGlasses).Id);
                EventHandler<EventArgs> refreshCustomersHandler = null;
                refreshCustomersHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshCustomersHandler;
                    this.FillGlassesList();
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
                this.FillGlassesList();
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
