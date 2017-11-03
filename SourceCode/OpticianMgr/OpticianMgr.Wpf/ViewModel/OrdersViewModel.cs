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
        public object SelectedLenses { get; set; }
        public ICommand OpenGlasses { get; set; }
        public ICommand FilterAndSortG { get; set; }
        public ICommand DeleteFilterG { get; set; }
        public ICommand OpenLenses { get; set; }
        public ICommand FilterAndSortC { get; set; }
        public ICommand DeleteFilterC { get; set; }
        public ObservableCollection<Order> Glasses { get; set; }
        public ICollectionView GlassesView { get; set; }
        public ObservableCollection<Order> ContactLenses { get; set; }
        public ICollectionView ContactLensesView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public string TranslatedGlassesSortProperty { get; set; }
        public string FilterGlassesProperty { get; set; }
        public string TranslatedGlassesFilterProperty { get; set; }
        public string FilterGlassesText { get; set; }
        public string SortGlassesProperty { get; set; }
        public string TranslatedLensesSortProperty { get; set; }
        public string FilterLensesProperty { get; set; }
        public string TranslatedLensesFilterProperty { get; set; }
        public string FilterLensesText { get; set; }
        public string SortLensesProperty { get; set; }
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
                props.Remove("GlassType_Id");
                props.Remove("EyeGlassFrame_Id");
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
        public ObservableCollection<String> LensesPropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Order).GetProperties().Select(c => c.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                //shouldnt be able to filter by these properties
                props.Remove("Timestamp");
                props.Remove("Customer_Id");
                props.Remove("Doctor_Id");
                props.Remove("Bill");
                props.Remove("OrderType");
                props.Remove("EyeGlassFrame_Id");
                props.Remove("ContactLensType_Id");
                props.Remove("GlassType");
                props.Remove("GlassTypeOthers");
                props.Remove("GlassType_Id");
                props.Remove("EyeGlassFrame");
 
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
            this.FilterGlassesProperty = "Kunde";
            this.SortLensesProperty = "Id";
            this.FilterLensesProperty = "Kunde";
            this.Glasses = GetAllGlassses();
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            this.ContactLenses = GetAllContactLenses();
            this.ContactLensesView = CollectionViewSource.GetDefaultView(ContactLenses);
            OpenGlasses = new RelayCommand(OpenG);
            FilterAndSortG = new RelayCommand(FilterAndSortGlasses);
            DeleteFilterG = new RelayCommand(DeleteGlassesF);
            OpenLenses = new RelayCommand(OpenC);
            FilterAndSortC = new RelayCommand(FilterAndSortContactLenses);
            DeleteFilterC = new RelayCommand(DeleteLensesF);
        }
        public void DeleteGlassesF()
        {
            this.FilterGlassesText = "";
            FilterAndSortGlasses();
            this.RaisePropertyChanged(() => this.FilterGlassesText);
        }
        public void DeleteLensesF()
        {
            this.FilterLensesText = "";
            FilterAndSortContactLenses();
            this.RaisePropertyChanged(() => this.FilterLensesText);
        }
        public void FillGlassesList()
        {
            this.Glasses = GetAllGlassses();
            this.RaisePropertyChanged(() => this.Glasses);
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            FilterAndSortGlasses();
            this.RaisePropertyChanged(() => this.GlassesView);
        }
        public void FillContactLensesList()
        {
            this.ContactLenses = GetAllContactLenses();
            this.RaisePropertyChanged(() => this.ContactLenses);
            this.ContactLensesView = CollectionViewSource.GetDefaultView(ContactLenses);
            FilterAndSortContactLenses();
            this.RaisePropertyChanged(() => this.ContactLensesView);
        }
        public void FilterAndSortGlasses()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Doktor" ? "Doktorname" : this.FilterGlassesProperty;
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Kunde" ? "Nachname" : this.FilterGlassesProperty;
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Brillenfassung" ? "Modell" : this.FilterGlassesProperty;
            this.FilterGlassesProperty = this.FilterGlassesProperty == "Glastyp" ? "Glastypbezeichnung" : this.FilterGlassesProperty;
            this.SortGlassesProperty = this.SortGlassesProperty == "Doktor" ? "Doktorname" : this.SortGlassesProperty;
            this.SortGlassesProperty = this.SortGlassesProperty == "Kunde" ? "Nachname" : this.SortGlassesProperty;
            this.SortGlassesProperty = this.SortGlassesProperty == "Brillenfassung" ? "Modell" : this.SortGlassesProperty;
            this.SortGlassesProperty = this.SortGlassesProperty == "Glastyp" ? "Glastypbezeichnung" : this.SortGlassesProperty;
            this.TranslatedGlassesFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterGlassesProperty).Key?.ToString();
            this.TranslatedGlassesSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortGlassesProperty).Key?.ToString();
            FilterGlasses();
            SortGlasses();
        }
        public void FilterAndSortContactLenses()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterLensesProperty = this.FilterLensesProperty == "Doktor" ? "Doktorname" : this.FilterLensesProperty;
            this.FilterLensesProperty = this.FilterLensesProperty == "Kunde" ? "Nachname" : this.FilterLensesProperty;
            this.FilterLensesProperty = this.FilterLensesProperty == "Kontaktlinsentyp" ? "Kontaktlinsentypbeschreibung" : this.FilterLensesProperty;
            this.SortLensesProperty = this.SortLensesProperty == "Doktor" ? "Doktorname" : this.SortLensesProperty;
            this.SortLensesProperty = this.SortLensesProperty == "Kunde" ? "Nachname" : this.SortLensesProperty;
            this.SortLensesProperty = this.SortLensesProperty == "Kontaktlinsentyp" ? "Kontaktlinsentypbeschreibung" : this.SortLensesProperty;
            this.TranslatedLensesFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterLensesProperty).Key?.ToString();
            this.TranslatedLensesSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortLensesProperty).Key?.ToString();
            FilterContactLenses();
            SortContactLenses();
        }
        public void SortGlasses()
        {
            try
            {
                this.GlassesView.SortDescriptions.Clear();
                if (typeof(Order).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription(this.TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Customer).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("Customer." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Doctor).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("Doctor." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(EyeGlassFrame).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("EyeGlassFrame." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Glasstype).GetProperty(TranslatedGlassesSortProperty) != null)
                    this.GlassesView.SortDescriptions.Add(new SortDescription("GlassType." + TranslatedGlassesSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void SortContactLenses()
        {
            try
            {
                this.ContactLensesView.SortDescriptions.Clear();
                if (typeof(Order).GetProperty(TranslatedLensesSortProperty) != null)
                    this.ContactLensesView.SortDescriptions.Add(new SortDescription(this.TranslatedLensesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Customer).GetProperty(TranslatedLensesSortProperty) != null)
                    this.ContactLensesView.SortDescriptions.Add(new SortDescription("Customer." + TranslatedLensesSortProperty, ListSortDirection.Ascending));
                else if (typeof(Doctor).GetProperty(TranslatedLensesSortProperty) != null)
                    this.ContactLensesView.SortDescriptions.Add(new SortDescription("Doctor." + TranslatedLensesSortProperty, ListSortDirection.Ascending));
                else if (typeof(ContactLensType).GetProperty(TranslatedLensesSortProperty) != null)
                    this.ContactLensesView.SortDescriptions.Add(new SortDescription("ContactLensType." + TranslatedLensesSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void FilterGlasses()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterGlassesText))
                {
                    this.GlassesView.Filter = new Predicate<object>(GlassesContains);
                }
                else
                    this.GlassesView.Filter = null;

            }
            catch (Exception e) { }
        }
        public void FilterContactLenses()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterLensesText))
                {
                    this.ContactLensesView.Filter = new Predicate<object>(ContactLensesContain);
                }
                else
                    this.ContactLensesView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool GlassesContains(object o)
        {
            Order order = o as Order;
            if (typeof(Order).GetProperty(TranslatedGlassesFilterProperty) != null)
            {
                return order.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(order, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(Customer).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return order.Customer?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(order.Customer, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(Doctor).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return order.Doctor?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(order.Doctor, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(EyeGlassFrame).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return order.EyeGlassFrame?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(order.EyeGlassFrame, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else if (typeof(Glasstype).GetProperty(TranslatedGlassesFilterProperty) != null) //Does the user filter by countryname?
            {
                return order.GlassType?.GetType().GetProperty(this.TranslatedGlassesFilterProperty).GetValue(order.GlassType, null)?.ToString().ToUpper().IndexOf(this.FilterGlassesText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        private bool ContactLensesContain(object o)
        {
            Order order = o as Order;
            if (typeof(Order).GetProperty(TranslatedLensesFilterProperty) != null)
            {
                return order.GetType().GetProperty(this.TranslatedLensesFilterProperty).GetValue(order, null)?.ToString().ToUpper().IndexOf(this.FilterLensesText.ToUpper()) >= 0;
            }
            else if (typeof(Customer).GetProperty(TranslatedLensesFilterProperty) != null) 
            {
                return order.Customer?.GetType().GetProperty(this.TranslatedLensesFilterProperty).GetValue(order.Customer, null)?.ToString().ToUpper().IndexOf(this.FilterLensesText.ToUpper()) >= 0;
            }
            else if (typeof(Doctor).GetProperty(TranslatedLensesFilterProperty) != null)
            {
                return order.Doctor?.GetType().GetProperty(this.TranslatedLensesFilterProperty).GetValue(order.Doctor, null)?.ToString().ToUpper().IndexOf(this.FilterLensesText.ToUpper()) >= 0;
            }
            else if (typeof(ContactLensType).GetProperty(TranslatedLensesFilterProperty) != null)
            {
                return order.ContactLensType?.GetType().GetProperty(this.TranslatedLensesFilterProperty).GetValue(order.ContactLensType, null)?.ToString().ToUpper().IndexOf(this.FilterLensesText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        //TODO
        public void OpenG()
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
        private ObservableCollection<Order> GetAllGlassses()
        {
            var unitofWorkGlasses = this.Uow.OrderRepository.Get(filter: o => o.OrderType == "B").ToList();
            ObservableCollection<Order> copiedGlasses = new ObservableCollection<Order>();
            foreach (var item in unitofWorkGlasses)
            {
                Order order = new Order();
                GenericRepository<Order>.CopyProperties(order, item);

                    Customer cutomer = new Customer();
                    GenericRepository<Customer>.CopyProperties(cutomer, this.Uow.CustomerRepository.GetById(item.Customer_Id));
                    order.Customer = cutomer;
                
                if (item.Doctor_Id != null)
                {
                    Doctor doctor = new Doctor();
                    GenericRepository<Doctor>.CopyProperties(doctor, this.Uow.DoctorRepository.GetById(item.Doctor_Id));
                    order.Doctor = doctor;
                }
                if (item.EyeGlassFrame_Id != null)
                {
                    EyeGlassFrame egf = new EyeGlassFrame();
                    GenericRepository<EyeGlassFrame>.CopyProperties(egf, this.Uow.EyeGlassFrameRepository.GetById(item.EyeGlassFrame_Id));
                    order.EyeGlassFrame = egf;
                }
                if (item.GlassType_Id != null)
                {
                    Glasstype glassType = new Glasstype();
                    GenericRepository<Glasstype>.CopyProperties(glassType, this.Uow.GlassTypeRepository.GetById(item.GlassType_Id));
                    order.GlassType = glassType;
                }
                copiedGlasses.Add(order);
            }
            return copiedGlasses;
        }
        private ObservableCollection<Order> GetAllContactLenses()
        {
            var unitofWorkContactLenses = this.Uow.OrderRepository.Get(filter: o => o.OrderType == "K").ToList();
            ObservableCollection<Order> copiedContactLenses = new ObservableCollection<Order>();
            foreach (var item in unitofWorkContactLenses)
            {
                Order order = new Order();
                GenericRepository<Order>.CopyProperties(order, item);

                Customer cutomer = new Customer();
                GenericRepository<Customer>.CopyProperties(cutomer, this.Uow.CustomerRepository.GetById(item.Customer_Id));
                order.Customer = cutomer;

                if (item.Doctor_Id != null)
                {
                    Doctor doctor = new Doctor();
                    GenericRepository<Doctor>.CopyProperties(doctor, this.Uow.DoctorRepository.GetById(item.Doctor_Id));
                    order.Doctor = doctor;
                }
                if (item.ContactLensType_Id != null)
                {
                    ContactLensType clt = new ContactLensType();
                    GenericRepository<ContactLensType>.CopyProperties(clt, this.Uow.ContactLensTypeRepository.GetById(item.ContactLensType_Id));
                    order.ContactLensType = clt;
                }
                copiedContactLenses.Add(order);
            }
            return copiedContactLenses;
        }
    }
}
