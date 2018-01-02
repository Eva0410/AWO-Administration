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
    public class AddGlasstypeViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Glasstype Glasstype { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddGlasstypeViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddGlasstype);
            Submit = new RelayCommand(AddGlasstype);
            this.InitFields();
        }
        public void CancelAddGlasstype()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddGlasstype()
        {
            try
            {
                if(this.Uow.GlassTypeRepository.Get(g => g.GlasstypeDescription == this.Glasstype.GlasstypeDescription).Count() > 0)
                    MessageBox.Show("Dieser Glastyp existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    this.Uow.GlassTypeRepository.Insert(this.Glasstype);
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
            this.Glasstype = new Glasstype();
            this.RaisePropertyChanged(() => this.Glasstype);
        }
    }
}
