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
        public static string[] PaymentStates
        {
            get { return new string[] { "Offen", "Bezahlt" }; }
        }
        public static string[] ProcessingStates
        {
            get { return new string[] { "Bestellt", "In Bearbeitung", "In Werkstatt", "Abgeholt" }; }
        }
        private IUnitOfWork Uow { get; set; }
        public object SelectedGlasses { get; set; }
        public object SelectedLenses { get; set; }
        public ICommand OpenGlasses { get; set; }
        public ICommand FilterAndSortG { get; set; }
        public ICommand DeleteFilterG { get; set; }
        public ICommand OpenLenses { get; set; }
        public ICommand FilterAndSortC { get; set; }
        public ICommand DeleteFilterC { get; set; }
        public ICommand SortCommandG { get; set; }
        public ICommand SortShiftG { get; set; }
        public ICommand InitializedG { get; set; }
        public SortManager SortManagerG { get; set; }
        public ICommand SortCommandC { get; set; }
        public ICommand SortShiftC { get; set; }
        public ICommand InitializedC { get; set; }
        public SortManager SortManagerC { get; set; }
        public ObservableCollection<Order> Glasses { get; set; }
        public ICollectionView GlassesView { get; set; }
        public ObservableCollection<Order> ContactLenses { get; set; }
        public ICollectionView ContactLensesView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        
        public string FilterGlassesProperty { get; set; }
        public string TranslatedGlassesFilterProperty { get; set; }
        public string FilterGlassesText { get; set; }
        public string FilterLensesProperty { get; set; }
        public string TranslatedLensesFilterProperty { get; set; }
        public string FilterLensesText { get; set; }
        public ObservableCollection<String> GlassesPropertiesList { get; }
        public ObservableCollection<String> LensesPropertiesList { get; }
        public OrdersViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.GlassesPropertiesList = GetAllGlassesProperties();
            this.LensesPropertiesList = GetAllLensesProperties();
            this.FilterGlassesProperty = "Kunde";
            this.FilterLensesProperty = "Kunde";
            this.Glasses = GetAllGlassses();
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            this.ContactLenses = GetAllContactLenses();
            this.ContactLensesView = CollectionViewSource.GetDefaultView(ContactLenses);
            OpenGlasses = new RelayCommand(OpenG);
            FilterAndSortG = new RelayCommand(FilterGlasses);
            DeleteFilterG = new RelayCommand(DeleteGlassesF);
            OpenLenses = new RelayCommand(OpenC);
            FilterAndSortC = new RelayCommand(FilterContactLenses);
            DeleteFilterC = new RelayCommand(DeleteLensesF);
            SortCommandG = new RelayCommand<RoutedEventArgs>(SortSG);
            SortShiftG = new RelayCommand<object>(SortShG);
            InitializedG = new RelayCommand<RoutedEventArgs>(InitG);
            SortManagerG = new SortManager();
            SortCommandC = new RelayCommand<RoutedEventArgs>(SortSC);
            SortShiftC = new RelayCommand<object>(SortShC);
            InitializedC = new RelayCommand<RoutedEventArgs>(InitC);
            SortManagerC = new SortManager();

            EventHandler<EventArgs> refreshGlassesOrders = null;
            refreshGlassesOrders = (sender, e) =>
            {
                this.FillGlassesList();
            };
            ViewModelLocator.AddGlassesOrderViewModel.Refresh += refreshGlassesOrders;
            ViewModelLocator.GlassesOrderDetailsViewModel.Refresh += refreshGlassesOrders;
            ViewModelLocator.GlassTypeDetailsViewModel.Refresh += refreshGlassesOrders;

            EventHandler<EventArgs> refreshContactLensOrders = null;
            refreshContactLensOrders = (sender, e) =>
            {
                this.FillContactLensesList();
            };
            ViewModelLocator.AddContactLensesOrderViewModel.Refresh += refreshContactLensOrders;
            ViewModelLocator.ContactLensOrderDetailsViewModel.Refresh += refreshContactLensOrders;
            ViewModelLocator.ContactLensTypeDetailsViewModel.Refresh += refreshContactLensOrders;
        }
        private ObservableCollection<string> GetAllGlassesProperties()
        {
            ObservableCollection<string> props = new ObservableCollection<string>(typeof(Order).GetProperties().Select(c => c.Name).ToList());
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
        private ObservableCollection<string> GetAllLensesProperties()
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
        private void InitG(RoutedEventArgs p)
        {
            SortManagerG.Init(p);
        }
        //Click without shift key
        private void SortSG(RoutedEventArgs e)
        {
            var tmp = this.GlassesView;
            SortManagerG.SortNormal(e, ref tmp);
        }

        //Click with shift
        private void SortShG(object p)
        {
            var tmp = this.GlassesView;
            SortManagerG.SortShift(p, ref tmp);
        }
        private void InitC(RoutedEventArgs p)
        {
            SortManagerC.Init(p);
        }
        //Click without shift key
        private void SortSC(RoutedEventArgs e)
        {
            var tmp = this.ContactLensesView;
            SortManagerC.SortNormal(e, ref tmp);
        }

        //Click with shift
        private void SortShC(object p)
        {
            var tmp = ContactLensesView;
            SortManagerC.SortShift(p, ref tmp);
        }
        public void DeleteGlassesF()
        {
            this.FilterGlassesText = "";
            FilterGlasses();
            this.RaisePropertyChanged(() => this.FilterGlassesText);
        }
        public void DeleteLensesF()
        {
            this.FilterLensesText = "";
            FilterContactLenses();
            this.RaisePropertyChanged(() => this.FilterLensesText);
        }
        public void FillGlassesList()
        {
            this.Glasses = GetAllGlassses();
            this.RaisePropertyChanged(() => this.Glasses);
            var sort = this.GlassesView.SortDescriptions;
            this.GlassesView = CollectionViewSource.GetDefaultView(Glasses);
            this.GlassesView.SortDescriptions.Clear();
            foreach (var item in sort)
            {
                this.GlassesView.SortDescriptions.Add(item);
            }
            FilterGlasses();
            this.RaisePropertyChanged(() => this.GlassesView);
        }
        public void FillContactLensesList()
        {
            this.ContactLenses = GetAllContactLenses();
            this.RaisePropertyChanged(() => this.ContactLenses);
            var sort = this.ContactLensesView.SortDescriptions;
            this.ContactLensesView = CollectionViewSource.GetDefaultView(ContactLenses);
            this.ContactLensesView.SortDescriptions.Clear();
            foreach (var item in sort)
            {
                this.ContactLensesView.SortDescriptions.Add(item);
            }
            FilterContactLenses();
            this.RaisePropertyChanged(() => this.ContactLensesView);
        }
        public void FilterGlasses()
        {
            this.TranslatedGlassesFilterProperty = TranslateGermanToEnglish(this.FilterGlassesProperty);
            FilterG();
        }
        private string TranslateGermanToEnglish(string germanName)
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            germanName = germanName == "Doktor" ? "Doktorname" : germanName;
            germanName = germanName == "Kunde" ? "Nachname" : germanName;
            germanName = germanName == "Brillenfassung" ? "Modell" : germanName;
            germanName = germanName == "Glastyp" ? "Glastypbezeichnung" : germanName;
            return dictionary.FirstOrDefault(e => e.Value.ToString() == germanName).Key?.ToString();
        }
        public void FilterContactLenses()
        {
            this.TranslatedLensesFilterProperty = TranslateGermanToEnglish(this.FilterLensesProperty);
            FilterC();
        }
        public void FilterG()
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
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        public void FilterC()
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
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
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
                GlassesOrderDetailsViewModel viewModel = ViewModelLocator.GlassesOrderDetailsViewModel;
                viewModel.InitOrder(((Order)this.SelectedGlasses).Id);
                EventHandler<EventArgs> refreshOrdersHandler = null;
                refreshOrdersHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshOrdersHandler;
                    this.FillGlassesList();
                };
                viewModel.Refresh += refreshOrdersHandler;
                windowService.ShowGlassesOrderDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst eine Brillenbestellung aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void OpenC()
        {
            if (this.SelectedLenses != null)
            {
                WindowService windowService = new WindowService();
                ContactLensOrderDetailsViewModel viewModel = ViewModelLocator.ContactLensOrderDetailsViewModel;
                viewModel.InitOrder(((Order)this.SelectedLenses).Id);
                EventHandler<EventArgs> refreshOrdersHandler = null;
                refreshOrdersHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshOrdersHandler;
                    this.FillContactLensesList();
                };
                viewModel.Refresh += refreshOrdersHandler;
                windowService.ShowContactLensOrderDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst eine Kontaktlinsenbestellung aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
