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
    public class AddContactLensTypeViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public ContactLensType ContactLensType { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddContactLensTypeViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddContactLensType);
            Submit = new RelayCommand(AddContactLensType);
            this.InitFields();
        }
        public void CancelAddContactLensType()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddContactLensType()
        {
            try
            {
                if(this.Uow.ContactLensTypeRepository.Get(c => c.ContactLensTypeDescription == this.ContactLensType.ContactLensTypeDescription).Count() > 0)
                    MessageBox.Show("Dieser Kontaktlinsentyp existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    this.Uow.ContactLensTypeRepository.Insert(this.ContactLensType);
                    this.Uow.Save();
                    this.CloseRequested?.Invoke(this, null);
                    this.Refresh?.Invoke(this, null);
                    this.InitFields();
                }
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
            this.ContactLensType = new ContactLensType();
            this.RaisePropertyChanged(() => this.ContactLensType);
        }
    }
}
