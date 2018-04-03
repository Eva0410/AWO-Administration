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
    public class TownDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public Town Town { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand Delete { get; set; }
        public TownDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditTown);
            Submit = new RelayCommand(SaveTown);    
            Delete = new RelayCommand(DeleteT);
        }
        public void InitTown(int id)
        {
            var uowTown = this.Uow.TownRepository.GetById(id);
            Town newTown = new Town();
            GenericRepository<Town>.CopyProperties(newTown, uowTown);
            this.Town = newTown;
            RaisePropertyChanged(() => this.Town);
        }

        public void DeleteT()
        {
            var result = MessageBox.Show(String.Format("Wollen Sie den Ort '{0}' wirklich löschen? {1} Achtung! {1} {2} Kunden und {3} Lieferanten verweisen auf diesen Ort. {1}Wenn Sie den Ort dennoch löschen bleibt bei diesen Kunden/Lieferant der Ort leer.", this.Town.TownName, Environment.NewLine, this.Uow.CustomerRepository.Count(filter: c => c.Town_Id == this.Town.Id), this.Uow.SupplierRepository.Count(filter: s => s.Town_Id == this.Town.Id)), "Ort löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var t = this.Uow.TownRepository.GetById(this.Town.Id);
                this.Uow.TownRepository.Delete(t);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
        }
        
        public void CancelEditTown()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
        }
        public void SaveTown()
        {
            try
            {
                this.Uow.TownRepository.Update(this.Town);
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

