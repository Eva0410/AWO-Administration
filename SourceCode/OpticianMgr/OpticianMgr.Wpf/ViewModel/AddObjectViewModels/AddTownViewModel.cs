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
    public class AddTownViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Town Town { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddTownViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddTown);
            Submit = new RelayCommand(AddTown);
            this.InitFields();
        }
        public void CancelAddTown()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddTown()
        {
            try
            {
                if(this.Uow.TownRepository.Get(t => t.TownName == this.Town.TownName && t.ZipCode == this.Town.ZipCode).Count() > 0)
                {
                    MessageBox.Show("Dieser Ort existert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    this.Uow.TownRepository.Insert(this.Town);
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
            this.Town = new Town();
            this.RaisePropertyChanged(() => this.Town);
        }
    }
}
