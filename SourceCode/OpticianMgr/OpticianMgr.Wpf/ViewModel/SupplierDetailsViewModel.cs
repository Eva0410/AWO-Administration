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
    public class SupplierDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> RefreshSuppliers;

        public Supplier Supplier { get; set; }
        public List<Town> Towns { get; set; }
        public List<Country> Countries { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddTown { get; set; }
        public ICommand AddCountry { get; set; }
        public ICommand Delete { get; set; }
        public SupplierDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelSave);
            Submit = new RelayCommand(SaveSupplier);
            AddTown = new RelayCommand(AddT);
            AddCountry = new RelayCommand(AddC);
            Delete = new RelayCommand(DeleteS);
        }
        public void InitSupplier(int id)
        {
            var sup = CopySupplier(this.Uow.SupplierRepository.GetById(id));
            this.Supplier = sup;

            InitFields();
            SetTownAndCountry();
            RaisePropertyChanged(() => this.Supplier);
        }
        private Supplier CopySupplier(Supplier item)
        {
            Supplier supplier = new Supplier();
            GenericRepository<Supplier>.CopyProperties(supplier, item);
            if (item.Town_Id != null)
            {
                Town town = new Town(); //Referenced town must be copied as well
                GenericRepository<Town>.CopyProperties(town, this.Uow.TownRepository.GetById(item.Town_Id));
                supplier.Town = town;
            }
            if (item.Country_Id != null)
            {
                Country country = new Country();
                GenericRepository<Country>.CopyProperties(country, this.Uow.CountryRepository.GetById(item.Country_Id));
                supplier.Country = country;
            }
            return supplier;
        }

        private void SetTownAndCountry()
        {
            this.Supplier.Town = this.Supplier.Town_Id == null ? Towns[0] : Towns.Where(t => t.Id == this.Supplier.Town_Id).FirstOrDefault();
            this.Supplier.Country = this.Supplier.Country_Id == null ? Countries[0] : Countries.Where(c => c.Id == this.Supplier.Country_Id).FirstOrDefault();
        }
        public void DeleteS()
        {
            var result = MessageBox.Show("Wollen Sie den Lieferantne '" + this.Supplier.Name + "' wirklich löschen?", "Lieferant löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var s = this.Uow.SupplierRepository.GetById(this.Supplier.Id);
                this.Uow.SupplierRepository.Delete(s);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.RefreshSuppliers?.Invoke(this, null);
                this.InitFields();
            }
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
        public void CancelSave()
        {
            this.CloseRequested?.Invoke(this, null);
            this.RefreshSuppliers?.Invoke(this, null);
            this.InitFields();
        }
        public void SaveSupplier()
        {
            if (this.Supplier.Town.Id == 0)
            {
                this.Supplier.Town_Id = null;
            }
            else
            {
                this.Supplier.Town_Id = this.Supplier.Town.Id;
            }
            if (this.Supplier.Country.Id == 0)
            {
                this.Supplier.Country_Id = null;
            }
            else
            {
                this.Supplier.Country_Id = this.Supplier.Country.Id;
            }
            this.Supplier.Town = null;
            this.Supplier.Country = null;
            try
            {
                this.Uow.SupplierRepository.Update(this.Supplier);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.RefreshSuppliers?.Invoke(this, null);
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
        }
        private void FillTowns()
        {
            var towns = this.Uow.TownRepository.Get(orderBy: o => o.OrderBy(t => t.ZipCode)).ToList();
            towns.Insert(0, new Town() { TownName = "Bitte wählen..." });
            this.Towns = towns;
            RaisePropertyChanged(() => this.Towns);

            this.Supplier.Town = this.Supplier.Town_Id == null ? Towns[0] : Towns.Where(t => t.Id == this.Supplier.Town_Id).FirstOrDefault();
            RaisePropertyChanged(() => this.Supplier);
        }
        private void FillCountries()
        {
            var countries = this.Uow.CountryRepository.Get(orderBy: o => o.OrderBy(c => c.CountryName)).ToList();
            countries.Insert(0, new Country() { CountryName = "Bitte wählen..." });
            this.Countries = countries;
            RaisePropertyChanged(() => this.Countries);

            this.Supplier.Country = this.Supplier.Country_Id == null ? Countries[0] : Countries.Where(c => c.Id == this.Supplier.Country_Id).FirstOrDefault();
            RaisePropertyChanged(() => this.Supplier);

        }
    }
}

