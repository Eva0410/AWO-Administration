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
    public class AddDoctorViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Doctor Doctor { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddDoctorViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddDoctor);
            Submit = new RelayCommand(AddDoctor);
            this.InitFields();
        }
        public void CancelAddDoctor()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddDoctor()
        {
            try
            {
                if(this.Uow.DoctorRepository.Get(d => d.DoctorName == this.Doctor.DoctorName).Count() > 0)
                    MessageBox.Show("Dieser Doktor existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    this.Uow.DoctorRepository.Insert(this.Doctor);
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
            this.Doctor = new Doctor();
            this.RaisePropertyChanged(() => this.Doctor);
        }
    }
}
