using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class AddCountryViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Country Country { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddCountryViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddCountry);
            Submit = new RelayCommand(AddCountry);
            this.InitFields();
        }
        public void CancelAddCountry()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddCountry()
        {
            try
            {
                //TODO Lieferanten-Ansicht anpassen und dann Validation Rules zum Land hinzufügen
                this.Uow.CountryRepository.Insert(this.Country);
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
            }
        }
        private void InitFields()
        {
            this.Country = new Country();
            this.RaisePropertyChanged(() => this.Country);
        }
    }
}
