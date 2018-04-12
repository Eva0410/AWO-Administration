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
    public class ContactLensTypeDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public ContactLensType ContactLensType { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand Delete { get; set; }
        public ContactLensTypeDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditContactLensType);
            Submit = new RelayCommand(SaveContactLensType);    
            Delete = new RelayCommand(DeleteC);
        }
        public void InitContactLensType(int id)
        {
            var uowContactLensType = this.Uow.ContactLensTypeRepository.GetById(id);
            ContactLensType newContactLensType = new ContactLensType();
            GenericRepository<ContactLensType>.CopyProperties(newContactLensType, uowContactLensType);
            this.ContactLensType = newContactLensType;
            RaisePropertyChanged(() => this.ContactLensType);
        }

        public void DeleteC()
        {
            var result = MessageBox.Show(String.Format("Wollen Sie den Kontaktlinsentyp '{0}' wirklich löschen? {1} Achtung! {1} {2} Bestellungen verweisen auf diesen Kontaktlinsentyp. {1}Wenn Sie den Kontaktlinsentyp dennoch löschen bleibt bei diesen Bestellungen der Kontaktlinsentyp leer.", this.ContactLensType.ContactLensTypeDescription, Environment.NewLine, this.Uow.OrderRepository.Count(filter: o => o.ContactLensType_Id == this.ContactLensType.Id)), "Kontaktlinsentyp löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var c = this.Uow.ContactLensTypeRepository.GetById(this.ContactLensType.Id);
                this.Uow.ContactLensTypeRepository.Delete(c);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
        }
        
        public void CancelEditContactLensType()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
        }
        public void SaveContactLensType()
        {
            try
            {
                this.Uow.ContactLensTypeRepository.Update(this.ContactLensType);
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

