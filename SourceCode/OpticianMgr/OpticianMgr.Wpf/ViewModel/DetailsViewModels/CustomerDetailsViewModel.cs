﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class CustomerDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public object SelectedOrder { get; set; }
        public List<Order> Orders { get; set; }
        public Customer Customer { get; set; }
        public List<Town> Towns { get; set; }
        public List<Country> Countries { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddTown { get; set; }
        public ICommand AddCountry { get; set; }
        public ICommand Delete { get; set; }
        public ICommand AddGlassesOrder { get; set; }
        public ICommand AddContactLensOrder { get; set; }
        public ICommand Show { get; set; }
        public ICommand ShowMessages { get; set; }
        public CustomerDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelSaveCustomer);
            Submit = new RelayCommand(SaveCustomer);
            AddTown = new RelayCommand(AddT);
            AddCountry = new RelayCommand(AddC);
            Delete = new RelayCommand(DeleteC);
            AddGlassesOrder = new RelayCommand(AddGO);
            AddContactLensOrder = new RelayCommand(AddCLO);
            ShowMessages = new RelayCommand(ShowM);
            Show = new RelayCommand(ShowOrder);
            EventHandler<EventArgs> refreshOrders = null;
            refreshOrders = (sender, e) =>
            {
                this.FillOrders();
            };
            ViewModelLocator.ContactLensOrderDetailsViewModel.Refresh += refreshOrders;
            ViewModelLocator.GlassesOrderDetailsViewModel.Refresh += refreshOrders;
        }
        public void InitCustomer(int id)
        {
            var cus = CopyCustomer(this.Uow.CustomerRepository.GetById(id));
            this.Customer = cus;

            InitFields();
            SetTownAndCountry();
            RaisePropertyChanged(() => this.Customer);
            RaisePropertyChanged(() => this.Orders);
        }
        private Customer CopyCustomer(Customer item)
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
            return customer;
        }
        private void SetTownAndCountry()
        {
            this.Customer.Town = this.Customer.Town_Id == null ? Towns[0] : Towns.Where(t => t.Id == this.Customer.Town_Id).FirstOrDefault();
            this.Customer.Country = this.Customer.Country_Id == null ? Countries[0] : Countries.Where(c => c.Id == this.Customer.Country_Id).FirstOrDefault();
        }

        public void DeleteC()
        {
            var result = MessageBox.Show(String.Format("Wollen Sie den Kunden {0} wirklich löschen? {1}Hinweis: Dadurch kann der Kunde zwar auf der Hauptseite angezeigt werden, aber er kann keine neuen Bestellugen mehr erhalten und nicht mehr bearbeitet werden.", this.Customer.LastName, Environment.NewLine), "Kunde löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var c = this.Uow.CustomerRepository.GetById(this.Customer.Id);
                c.Deleted = true;
                c.NewsLetter = false;
                c.Town = null;
                c.Country = null;
                this.Uow.CustomerRepository.Update(c);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
                this.InitFields();
            }
            //No customer should be deleted completely
            //var result = MessageBox.Show("Wollen Sie den Kunden '" + this.Customer.LastName + "' wirklich löschen?", "Kunde löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (MessageBoxResult.Yes == result)
            //{
            //    var c = this.Uow.CustomerRepository.GetById(this.Customer.Id);
            //    this.Uow.CustomerRepository.Delete(c);
            //    this.Uow.Save();
            //    this.CloseRequested?.Invoke(this, null);
            //    this.Refresh?.Invoke(this, null);
            //    this.InitFields();
            //}
        }
        public void AddT()
        {
            WindowService windowService = new WindowService();
            AddTownViewModel viewModel = ViewModelLocator.AddTownViewModel;
            EventHandler<EventArgs> refreshTownsEventHandler = null;
            refreshTownsEventHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshTownsEventHandler;
                this.FillTowns();
            };
            viewModel.Refresh += refreshTownsEventHandler;
            windowService.ShowAddTownWindow(viewModel);
        }
        public void AddC()
        {
            WindowService windowService = new WindowService();
            AddCountryViewModel viewModel = ViewModelLocator.AddCountryViewModel;
            EventHandler<EventArgs> refreshCountriesEventHandler = null;
            refreshCountriesEventHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshCountriesEventHandler;
                this.FillCountries();
            };
            viewModel.Refresh += refreshCountriesEventHandler;
            windowService.ShowAddCountryWindow(viewModel);
        }
        public void CancelSaveCustomer()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
            this.InitFields();
        }
        public void SaveCustomer()
        {
            if (this.Customer.Town.Id == 0)
            {
                this.Customer.Town_Id = null;
            }
            else
            {
                this.Customer.Town_Id = this.Customer.Town.Id;
            }
            if (this.Customer.Country.Id == 0)
            {
                this.Customer.Country_Id = null;
            }
            else
            {
                this.Customer.Country_Id = this.Customer.Country.Id;
            }
            this.Customer.Town = null;
            this.Customer.Country = null;
            try
            {
                this.Uow.CustomerRepository.Update(this.Customer);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += validationError.ErrorMessage;
                        message += "\n";
                    }
                }
                MessageBox.Show(message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                SetTownAndCountry();
            }
        }
        private void InitFields()
        {
            FillTowns();
            FillCountries();
            FillOrders();
        }
        private void FillTowns()
        {
            var towns = this.Uow.TownRepository.Get(orderBy: o => o.OrderBy(t => t.ZipCode)).ToList();
            towns.Insert(0, new Town() { TownName = "Bitte wählen..." });
            this.Towns = towns;
            RaisePropertyChanged(() => this.Towns);

            this.Customer.Town = this.Customer.Town_Id == null ? Towns[0] : Towns.Where(t => t.Id == this.Customer.Town_Id).FirstOrDefault();
            RaisePropertyChanged(() => this.Customer);
        }
        private void FillCountries()
        {
            var countries = this.Uow.CountryRepository.Get(orderBy: o => o.OrderBy(c => c.CountryName)).ToList();
            countries.Insert(0, new Country() { CountryName = "Bitte wählen..." });
            this.Countries = countries;
            RaisePropertyChanged(() => this.Countries);

            this.Customer.Country = this.Customer.Country_Id == null ? Countries[0] : Countries.Where(c => c.Id == this.Customer.Country_Id).FirstOrDefault();
            RaisePropertyChanged(() => this.Customer);
        }
        private void FillOrders()
        {
            this.Orders = this.Uow.OrderRepository.Get(filter: o => o.Customer_Id == this.Customer.Id).ToList();
            this.RaisePropertyChanged(() => this.Orders);
        }
        public void AddGO()
        {
            WindowService windowService = new WindowService();
            AddGlassesOrderViewModel viewModel = ViewModelLocator.AddGlassesOrderViewModel;
            viewModel.InitCustomer(this.Customer.Id);
            EventHandler<EventArgs> refreshGlassesOrdersEventHandler = null;
            refreshGlassesOrdersEventHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshGlassesOrdersEventHandler;
                FillOrders();
                RaisePropertyChanged(() => this.Orders);
            };
            viewModel.Refresh += refreshGlassesOrdersEventHandler;
            windowService.ShowAddGlassesOrderWindow(viewModel);
        }
        public void AddCLO()
        {
            WindowService windowService = new WindowService();
            AddContactLensesOrderViewModel viewModel = ViewModelLocator.AddContactLensesOrderViewModel;
            viewModel.InitCustomer(this.Customer.Id);
            EventHandler<EventArgs> refreshContactLensOrdersEventHandler = null;
            refreshContactLensOrdersEventHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshContactLensOrdersEventHandler;
                FillOrders();
                RaisePropertyChanged(() => this.Orders);
            };
            viewModel.Refresh += refreshContactLensOrdersEventHandler;
            windowService.ShowAddContactLensOrderWindow(viewModel);
        }
        public void ShowOrder()
        {
            if (this.SelectedOrder != null)
            {
                var order = (Order)this.SelectedOrder;
                if (order.OrderType == "B")
                {
                    WindowService windowService = new WindowService();
                    GlassesOrderDetailsViewModel viewModel = ViewModelLocator.GlassesOrderDetailsViewModel;
                    viewModel.InitOrder(order.Id);
                    EventHandler<EventArgs> refreshOrdersHandler = null;
                    refreshOrdersHandler = (sender, e) =>
                    {
                        viewModel.Refresh -= refreshOrdersHandler;
                        this.FillOrders();
                    };
                    viewModel.Refresh += refreshOrdersHandler;
                    windowService.ShowGlassesOrderDetailsWindow(viewModel);
                }
                else if (order.OrderType == "K")
                {
                    WindowService windowService = new WindowService();
                    ContactLensOrderDetailsViewModel viewModel = ViewModelLocator.ContactLensOrderDetailsViewModel;
                    viewModel.InitOrder(order.Id);
                    EventHandler<EventArgs> refreshOrdersHandler = null;
                    refreshOrdersHandler = (sender, e) =>
                    {
                        viewModel.Refresh -= refreshOrdersHandler;
                        this.FillOrders();
                    };
                    viewModel.Refresh += refreshOrdersHandler;
                    windowService.ShowContactLensOrderDetailsWindow(viewModel);
                }
            }

            else
                MessageBox.Show("Bitte wählen Sie zuerst eine Bestellung aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void ShowM()
        {
            WindowService windowService = new WindowService();
            SentMessagesViewModel viewModel = ViewModelLocator.SentMessagesViewModel;
            var messages = this.Uow.MessageRepository.Get(filter: m => m.Recipients.Where(r => r.Customer.Id == this.Customer.Id).FirstOrDefault() != null, orderBy: ord => ord.OrderByDescending(m => m.Date), includeProperties: "Recipients").ToList();
            viewModel.InitMessages(messages);
            windowService.ShowSentMessagesWindow(viewModel);
        }
    }
}

