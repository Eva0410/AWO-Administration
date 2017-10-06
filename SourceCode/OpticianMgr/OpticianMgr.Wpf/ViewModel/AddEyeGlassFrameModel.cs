using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ICommand FindSupplier { get; set; }
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
            this.Uow.EyeGlassFrameRepository.Insert(this.EyeGlassFrame);
            this.Uow.Save();
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
            this.InitFields();
        }
        private void InitFields()
        {
            this.EyeGlassFrame = new EyeGlassFrame();
            this.EyeGlassFrame.State = States[0];
            var suppliers = this.Uow.SupplierRepository.Get(orderBy: o => o.OrderBy(s => s.Name)).ToList();
            suppliers.Insert(0, new Supplier() { Name = "Bitte wählen..." });
            this.Suppliers = suppliers;
            this.EyeGlassFrame.Supplier = Suppliers[0];
            this.RaisePropertyChanged(() => this.EyeGlassFrame);
        }
    }
}

