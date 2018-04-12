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
    public class EyeGlassFramesDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public EyeGlassFrame EyeGlassFrame { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public string[] States { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand Delete { get; set; }
        public EyeGlassFramesDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddEyeGlassFrame);
            Submit = new RelayCommand(SaveEyeGlassFrame);
            Delete = new RelayCommand(DeleteE);
        }
        public void InitEyeGlassFrame(int id)
        {
            var egf = CopyEyeGlassFrame(this.Uow.EyeGlassFrameRepository.GetById(id));
            this.EyeGlassFrame = egf;

            InitFields();
            SetSupplier();
            RaisePropertyChanged(() => this.EyeGlassFrame);
        }
        private EyeGlassFrame CopyEyeGlassFrame(EyeGlassFrame item)
        {
            EyeGlassFrame efg = new EyeGlassFrame();
            GenericRepository<EyeGlassFrame>.CopyProperties(efg, item);
            if (item.Supplier_Id != null)
            {
                Supplier supplier = new Supplier(); //Referenced supplier must be copied as well
                GenericRepository<Supplier>.CopyProperties(supplier, this.Uow.SupplierRepository.GetById(item.Supplier_Id));
                efg.Supplier = supplier;
            }
            return efg;
        }
        private void SetSupplier()
        {
            this.EyeGlassFrame.Supplier = this.EyeGlassFrame.Supplier_Id == null ? Suppliers[0] : Suppliers.Where(t => t.Id == this.EyeGlassFrame.Supplier_Id).FirstOrDefault();
        }
        public void DeleteE()
        {
            var result = MessageBox.Show("Wollen Sie die Brillenfassung '" + this.EyeGlassFrame.ModelDescription + "' wirklich löschen?", "Brillenfassung löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var e = this.Uow.EyeGlassFrameRepository.GetById(this.EyeGlassFrame.Id);
                this.Uow.EyeGlassFrameRepository.Delete(e);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
                this.InitFields();
            }
        }
        public void CancelAddEyeGlassFrame()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
            this.InitFields();
        }
        public void SaveEyeGlassFrame()
        {
            if (this.EyeGlassFrame.Supplier.Id == 0)
            {
                this.EyeGlassFrame.Supplier = null;
            }
            else
            {
                this.EyeGlassFrame.Supplier_Id = this.EyeGlassFrame.Supplier.Id;
            }
            this.EyeGlassFrame.Supplier = null;
            try
            {
                this.Uow.EyeGlassFrameRepository.Update(this.EyeGlassFrame);
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
                SetSupplier();
            }
        }
        private void InitFields()
        {
            FillSuppliers();
            this.States = EyeGlassFramesViewModel.States;
            
        }
        private void FillSuppliers()
        {
            var suppliers = this.Uow.SupplierRepository.Get(orderBy: o => o.OrderBy(s => s.Name)).ToList();
            suppliers.Insert(0, new Supplier() { Name = "Bitte wählen..." });
            this.Suppliers = suppliers;
            RaisePropertyChanged(() => this.Suppliers);

            this.EyeGlassFrame.Supplier = this.EyeGlassFrame.Supplier_Id == null ? Suppliers[0] : Suppliers.Where(t => t.Id == this.EyeGlassFrame.Supplier_Id).FirstOrDefault();
            RaisePropertyChanged(() => this.EyeGlassFrame);
        }
    }
}

