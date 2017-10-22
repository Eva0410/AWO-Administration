﻿using GalaSoft.MvvmLight;
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
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class CustomerViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenCustomer { get; set; }
        public ICommand AddCustomer { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public CustomerViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            FillList();
            OpenCustomer = new RelayCommand(OpenS);
            AddCustomer = new RelayCommand(AddC);
        }
        public void FillList()
        {
            this.Customers = GetAllCustomers();
            this.RaisePropertyChanged(() => this.Customers);
        }
        public void OpenS()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                CustomerDetailsViewModel viewModel = ViewModelLocator.CustomerDetailsViewModel;
                viewModel.Customer = CopyCustomer((Customer)this.Selected);
                viewModel.RaisePropertyChanged(() => viewModel.Customer);
                viewModel.RaisePropertyChanged(() => viewModel.Customer.Town);
                viewModel.RaisePropertyChanged(() => viewModel.Customer.Country);
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
                copiedCustomers.Add(CopyCustomer(item));
            }
            return copiedCustomers;
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
    }
}
