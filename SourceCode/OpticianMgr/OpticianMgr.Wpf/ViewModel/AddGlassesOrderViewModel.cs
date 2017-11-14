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
    public class AddGlassesOrderViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Order Order { get; set; }
        public List<Doctor> Doctors { get; set; }
        public string[] ProcessingStates { get; set; }
        public string[] PaymentStates { get; set; }
        public List<Glasstype> GlassTypes { get; set; }
        public List<EyeGlassFrame> EyeGlassFrames { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddDoctor { get; set; }
        public ICommand Calc { get; set; }
        public AddGlassesOrderViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddGlassesOrder);
            Submit = new RelayCommand(AddGlassesOrder);
            AddDoctor = new RelayCommand(AddD);
            Calc = new RelayCommand(Calculate);
            this.ProcessingStates = OrdersViewModel.ProcessingStates;
            this.PaymentStates = OrdersViewModel.PaymentStates;
            this.InitFields();
        }
        public void InitCustomer(int id)
        {
            var cus = this.Uow.CustomerRepository.GetById(id);
            this.Order.Customer = cus;
            this.Order.Customer_Id = id;
            SetFields();
            RaisePropertyChanged(() => this.Order);
        }
        public void AddD()
        {
            WindowService windowService = new WindowService();
            AddDoctorViewModel viewModel = ViewModelLocator.AddDoctorViewModel;
            EventHandler<EventArgs> refreshDoctorsEventHandler = null;
            refreshDoctorsEventHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshDoctorsEventHandler;
                this.FillDoctors();
            };
            viewModel.Refresh += refreshDoctorsEventHandler;
            windowService.ShowAddDoctorWindow(viewModel);
        }
        public void CancelAddGlassesOrder()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddGlassesOrder()
        {
            this.Order.Customer = null;
            this.Order.OrderType = "B";
            if (this.Order.EyeGlassFrame.Id == 0)
                this.Order.EyeGlassFrame = null;
            if (this.Order.GlassType.Id == 0)
                this.Order.GlassType = null;
            if (this.Order.Doctor.Id == 0)
                this.Order.Doctor = null;
            try
            {
                this.Uow.OrderRepository.Insert(this.Order);
                this.Uow.Save();
                //for debugging
                //foreach (var prop in typeof(Order).GetProperties())
                //{
                //    Console.WriteLine(prop.Name + ": " + this.Order.GetType().GetProperty(prop.Name).GetValue(this.Order, null));
                //}
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
        //TODO Calculate discount patientscontribution insuranceprice
        private void Calculate()
        {
            decimal tmp = Order.GlassPriceLeft + Order.GlassPriceRight + Convert.ToDecimal(Order.OthersPrice);
            this.Order.NetPrice = tmp;
            this.Order.GrossPrice = tmp * Convert.ToDecimal(1.2);
            RaisePropertyChanged(() => this.Order);
        }
        private void InitFields()
        {
            this.Order = new Order();
            SetFields();
        }
        private void FillDoctors()
        {
            var docs = this.Uow.DoctorRepository.Get(orderBy: o => o.OrderBy(d => d.DoctorName)).ToList();
            docs.Insert(0, new Doctor() { DoctorName = "Bitte wählen..." });
            this.Doctors = docs;
            RaisePropertyChanged(() => this.Doctors);

            this.Order.Doctor = Doctors[0];
            RaisePropertyChanged(() => this.Order);
        }
        private void FillGlassTypes()
        {
            var glasstypes = this.Uow.GlassTypeRepository.Get(orderBy: o => o.OrderBy(g => g.GlasstypeDescription)).ToList();
            glasstypes.Insert(0, new Glasstype() { GlasstypeDescription = "Bitte wählen..." });
            this.GlassTypes = glasstypes;
            RaisePropertyChanged(() => this.GlassTypes);

            this.Order.GlassType = GlassTypes[0];
            RaisePropertyChanged(() => this.Order);
        }
        private void FillEyeGlassFrames()
        {
            var egf = this.Uow.EyeGlassFrameRepository.Get(orderBy: o => o.OrderBy(e => e.ModelDescription)).ToList();
            egf.Insert(0, new EyeGlassFrame() { ModelDescription = "Bitte wählen..." });
            this.EyeGlassFrames = egf;
            RaisePropertyChanged(() => this.EyeGlassFrames);

            this.Order.EyeGlassFrame = EyeGlassFrames[0];
            RaisePropertyChanged(() => this.Order);
        }
        private void SetFields()
        {
            this.Order.PaymentState = this.PaymentStates[0];
            this.Order.ProcessingState = this.ProcessingStates[0];
            this.Order.GlassPriceLeft = 0;
            this.Order.GlassPriceRight = 0;
            FillDoctors();
            FillGlassTypes();
            FillEyeGlassFrames();
            this.Order.PaymentDate = null;
            this.Order.OrderDate = null;
            this.RaisePropertyChanged(() => this.Order);
        }
    }
}
