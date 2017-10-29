using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class AddSupplierViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow  { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> RefreshSuppliers;

        public Supplier Supplier { get; set; }
        public List<Town> Towns { get; set; }
        public List<Country> Countries { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddTown { get; set; }
        public ICommand AddCountry { get; set; }
        public AddSupplierViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddSupplier);
            Submit = new RelayCommand(AddSupplier);
            AddTown = new RelayCommand(AddT);
            AddCountry = new RelayCommand(AddC);
            this.InitFields();
        }
        private void AddT()
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
        private void AddC()
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
        public void CancelAddSupplier()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddSupplier()
        {
            if (this.Supplier.Town.Id == 0)
                this.Supplier.Town = null;
            if (this.Supplier.Country.Id == 0)
                this.Supplier.Country = null;
            try
            {
                this.Uow.SupplierRepository.Insert(this.Supplier);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.RefreshSuppliers?.Invoke(this, null);
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
                this.Supplier.Town = Towns[0];
                this.Supplier.Country = Countries[0];
            }
        }
        private void InitFields()
        {
            this.Supplier = new Supplier();
            FillTowns();
            FillCountries();
            this.RaisePropertyChanged(() => this.Supplier);
        }
        private void FillTowns()
        {
            var towns = this.Uow.TownRepository.Get(orderBy: o => o.OrderBy(t => t.ZipCode)).ToList();
            towns.Insert(0, new Town() { TownName = "Bitte wählen..." });
            this.Towns = towns;
            RaisePropertyChanged(() => this.Towns);

            this.Supplier.Town = Towns[0];
            RaisePropertyChanged(() => this.Supplier);
        }
        private void FillCountries()
        {
            var countries = this.Uow.CountryRepository.Get(orderBy: o => o.OrderBy(c => c.CountryName)).ToList();
            countries.Insert(0, new Country() { CountryName = "Bitte wählen..." });
            this.Countries = countries;
            RaisePropertyChanged(() => this.Countries);

            this.Supplier.Country = Countries[0];
            RaisePropertyChanged(() => this.Supplier);

        }
    }
}
