using GalaSoft.MvvmLight;
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
        public Town Town { get; set; }
        private Customer myVar;

        public Customer Customer
        {
            get { return myVar; }
            set { myVar = value;
                this.Town = Customer.Town;
                RaisePropertyChanged(() => Town);
                RaisePropertyChanged(() => Customer.Country);
                RaisePropertyChanged(() => this.Customer);
            }
        }

        public List<Town> Towns { get; set; }
        public List<Country> Countries { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddTown { get; set; }
        public ICommand AddCountry { get; set; }
        public ICommand Delete { get; set; }
        public CustomerDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddCustomer);
            Submit = new RelayCommand(AddCustomer);
            AddTown = new RelayCommand(AddT);
            AddCountry = new RelayCommand(AddC);
            Delete = new RelayCommand(DeleteC);
            this.InitFields();
        }
        public void DeleteC()
        {
            var result = MessageBox.Show("Wollen Sie den Kunden '" + this.Customer.LastName + "' wirklich löschen?", "Kunde löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(MessageBoxResult.Yes == result)
            {
                var c = this.Uow.CustomerRepository.GetById(this.Customer.Id);
                this.Uow.CustomerRepository.Delete(c);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
                this.InitFields();
            }
        }
        //TODO Ort und Land werden nicht angezeigt
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
        public void CancelAddCustomer()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
            this.InitFields();
        }
        public void AddCustomer()
        {
            if (this.Customer.Town.Id == 0)
                this.Customer.Town = null;
            if (this.Customer.Country.Id == 0)
                this.Customer.Country = null;
            try
            {
                this.Uow.CustomerRepository.Insert(this.Customer);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
                this.InitFields();
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
                this.Customer.Town = Towns[0];
                this.Customer.Country = Countries[0];
            }
        }
        private void InitFields()
        {
            FillTowns();
            FillCountries();
        }
        private void FillTowns()
        {
            var towns = this.Uow.TownRepository.Get(orderBy: o => o.OrderBy(t => t.ZipCode)).ToList();
            towns.Insert(0, new Town() { TownName = "Bitte wählen..." });
            this.Towns = towns;
            RaisePropertyChanged(() => this.Towns);
        }
        private void FillCountries()
        {
            var countries = this.Uow.CountryRepository.Get(orderBy: o => o.OrderBy(c => c.CountryName)).ToList();
            countries.Insert(0, new Country() { CountryName = "Bitte wählen..." });
            this.Countries = countries;
            RaisePropertyChanged(() => this.Countries);
        }
    }
}

