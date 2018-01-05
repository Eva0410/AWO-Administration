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
        public bool ShowDeleted { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand SortShift { get; set; }
        public ICommand Initialized { get; set; }
        public SortManager SortManager { get; set; }
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
            this.FilterProperty = "Nachname";
            this.Customers = GetAllCustomers();
            this.CustomersView = CollectionViewSource.GetDefaultView(Customers);
            this.FilterCustomers();
            OpenCustomer = new RelayCommand(OpenC);
            AddCustomer = new RelayCommand(AddC);
            FilterAndSort = new RelayCommand(FilterCustomers);
            DeleteFilter = new RelayCommand(DeleteF);
            this.ShowDeleted = false;
            SortCommand = new RelayCommand<RoutedEventArgs>(SortS);
            SortShift = new RelayCommand<object>(SortSh);
            Initialized = new RelayCommand<RoutedEventArgs>(Init);
            SortManager = new SortManager();

            EventHandler<EventArgs> refreshCustomers = null;
            refreshCustomers = (sender, e) =>
            {
                this.FillList();
            };
            ViewModelLocator.TownDetailsViewModel.Refresh += refreshCustomers;
            ViewModelLocator.CountryDetailsViewModel.Refresh += refreshCustomers;
        }
        private void Init(RoutedEventArgs p)
        {
            SortManager.Init(p);
        }
        //Click without shift key
        private void SortS(RoutedEventArgs e)
        {
            var tmp = this.CustomersView;
            SortManager.SortNormal(e, ref tmp);
        }

        //Click with shift
        private void SortSh(object p)
        {
            var tmp = CustomersView;
            SortManager.SortShift(p, ref tmp);
        }
        public void DeleteF()
        {
            this.FilterText = "";
            FilterCustomers();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Customers = GetAllCustomers();
            this.RaisePropertyChanged(() => this.Customers);
            var sort = this.CustomersView.SortDescriptions;
            this.CustomersView = CollectionViewSource.GetDefaultView(Customers);
            this.CustomersView.SortDescriptions.Clear();
            foreach (var item in sort)
            {
                this.CustomersView.SortDescriptions.Add(item);
            }
            FilterCustomers();
            this.RaisePropertyChanged(() => this.CustomersView);
        }
        public void FilterCustomers()
        {
            this.TranslatedFilterProperty = TranslateGermanToEnglish(this.FilterProperty);
            Filter();
        }
        private string TranslateGermanToEnglish(string germanName)
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            germanName = germanName == "Ort" ? "Ortsname" : germanName; //Program filters by the name of the town and not by by town-object
            germanName = germanName == "Land" ? "Landname" : germanName;
            return dictionary.FirstOrDefault(e => e.Value.ToString() == germanName).Key?.ToString();
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
                    this.CustomersView.Filter = new Predicate<object>(IsCustomerShown);

            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }
        private bool IsCustomerShown(object c)
        {
            Customer customer = c as Customer;
            if (!(customer.Deleted == true && false == this.ShowDeleted))
                return true;
            return false;
        }
        private bool Contains(object c)
        {
            Customer customer = c as Customer;

            if(IsCustomerShown(c))
            {
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
            else {
                return false;
            }
        }
        public void OpenC()
        {
            if (this.Selected != null)
            {
                if (((Customer)this.Selected).Deleted)
                {
                    MessageBox.Show("Dieser Kunde wurde bereits gelöscht!", "Kunde gelöscht", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
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
