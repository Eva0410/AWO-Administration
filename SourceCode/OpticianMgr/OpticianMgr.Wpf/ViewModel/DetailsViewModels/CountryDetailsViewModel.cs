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
    public class CountryDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public Country Country { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand Delete { get; set; }
        public CountryDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditCountry);
            Submit = new RelayCommand(SaveCountry);    
            Delete = new RelayCommand(DeleteC);
        }
        public void InitCountry(int id)
        {
            var uowCountry = this.Uow.CountryRepository.GetById(id);
            Country newCountry = new Country();
            GenericRepository<Country>.CopyProperties(newCountry, uowCountry);
            this.Country = newCountry;
            RaisePropertyChanged(() => this.Country);
        }

        public void DeleteC()
        {
            var result = MessageBox.Show(String.Format("Wollen Sie das Land '{0}' wirklich löschen? {1} Achtung! {1} {2} Kunden und {3} Lieferanten verweisen auf dieses Land. {1}Wenn Sie das Land dennoch löschen bleibt bei diesen Kunden/Lieferant das Land leer.", this.Country.CountryName, Environment.NewLine, this.Uow.CustomerRepository.Count(filter: c => c.Country_Id == this.Country.Id), this.Uow.SupplierRepository.Count(filter: s => s.Country_Id == this.Country.Id)), "Land löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var c = this.Uow.CountryRepository.GetById(this.Country.Id);
                this.Uow.CountryRepository.Delete(c);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
        }
        
        public void CancelEditCountry()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
        }
        public void SaveCountry()
        {
            try
            {
                this.Uow.CountryRepository.Update(this.Country);
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
            }
        }
        
    }
}

