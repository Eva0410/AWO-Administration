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
    public class AddEyeGlassFrameModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public EyeGlassFrame EyeGlassFrame { get; set; }
        public string[] States { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public AddEyeGlassFrameModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddEyeGlassFrame);
            Submit = new RelayCommand(AddEyeGlassFrame);
            States = EyeGlassFramesViewModel.States;
            this.InitFields();
        }
        public void CancelAddEyeGlassFrame()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddEyeGlassFrame()
        {
            if (this.EyeGlassFrame.Supplier.Id == 0)
                this.EyeGlassFrame.Supplier = null;
            try
            {
                this.Uow.EyeGlassFrameRepository.Insert(this.EyeGlassFrame);
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
                this.EyeGlassFrame.Supplier = Suppliers[0];
            }
        }
        private void InitFields()
        {
            this.EyeGlassFrame = new EyeGlassFrame();
            this.EyeGlassFrame.State = States[0];
            FillSuppliers();
            this.EyeGlassFrame.SaleDate = null;
            this.EyeGlassFrame.PurchaseDate = null;
            this.RaisePropertyChanged(() => this.EyeGlassFrame);
        }
        private void FillSuppliers()
        {
            var suppliers = this.Uow.SupplierRepository.Get(orderBy: o => o.OrderBy(s => s.Name)).ToList();
            suppliers.Insert(0, new Supplier() { Name = "Bitte wählen..." });
            this.Suppliers = suppliers;
            RaisePropertyChanged(() => this.Suppliers);

            this.EyeGlassFrame.Supplier = Suppliers[0];
            RaisePropertyChanged(() => this.Suppliers);
        }
    }
}

