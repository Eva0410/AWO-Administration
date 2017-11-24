using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
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
    public class GlassesOrderDetailsViewModel : ViewModelBase, IRequestClose
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
        public ICommand Delete { get; set; }
        public ICommand SendMessage { get; set; }
        public GlassesOrderDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditGlassesOrder);
            Submit = new RelayCommand(EditGlassesOrder);
            AddDoctor = new RelayCommand(AddD);
            Calc = new RelayCommand(Calculate);
            Delete = new RelayCommand(DeleteGO);
            SendMessage = new RelayCommand(OpenSendMessageWindow);
            this.ProcessingStates = OrdersViewModel.ProcessingStates;
            this.PaymentStates = OrdersViewModel.PaymentStates;
        }
        public void InitOrder(int id)
        {
            this.Order = CopyOrder(this.Uow.OrderRepository.GetById(id));

            SetFields();
            SetForeignKeys();
            RaisePropertyChanged(() => this.Order);
        }
        private Order CopyOrder(Order item)
        {
            Order order = new Order();
            GenericRepository<Order>.CopyProperties(order, item);
            if (item.EyeGlassFrame_Id != null)
            {
                EyeGlassFrame egf = new EyeGlassFrame(); //Referenced objects must be copied as well
                GenericRepository<EyeGlassFrame>.CopyProperties(egf, this.Uow.EyeGlassFrameRepository.GetById(item.EyeGlassFrame_Id));
                order.EyeGlassFrame = egf;
            }
            if (item.GlassType_Id != null)
            {
                Glasstype glasstype = new Glasstype();
                GenericRepository<Glasstype>.CopyProperties(glasstype, this.Uow.GlassTypeRepository.GetById(item.GlassType_Id));
                order.GlassType = glasstype;
            }
            if (item.Doctor_Id != null)
            {
                Doctor doctor = new Doctor();
                GenericRepository<Doctor>.CopyProperties(doctor, this.Uow.DoctorRepository.GetById(item.Doctor_Id));
                order.Doctor = doctor;
            }
            return order;
        }
        private void SetForeignKeys()
        {
            this.Order.Doctor = this.Order.Doctor_Id == null ? Doctors[0] : Doctors.Where(d => d.Id == this.Order.Doctor_Id).FirstOrDefault();
            this.Order.EyeGlassFrame = this.Order.EyeGlassFrame_Id == null ? EyeGlassFrames[0] : EyeGlassFrames.Where(e => e.Id == this.Order.EyeGlassFrame_Id).FirstOrDefault();
            this.Order.GlassType = this.Order.GlassType_Id== null ? GlassTypes[0] : GlassTypes.Where(g => g.Id == this.Order.GlassType_Id).FirstOrDefault();
        }
        public void DeleteGO()
        {
            var result = MessageBox.Show("Wollen Sie die Bestellung vom Kunden '" + this.Order.Customer.LastName + "' wirklich löschen?", "Bestellung löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                this.Uow.OrderRepository.Delete(this.Order.Id);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
                this.SetFields();
            }
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
        public void CancelEditGlassesOrder()
        {
            this.CloseRequested?.Invoke(this, null);
            this.SetFields();
        }
        public void OpenSendMessageWindow()
        {
            WindowService windowService = new WindowService();
            SingleMessageViewModel viewModel = ViewModelLocator.SingleMessageViewModel;
            viewModel.OrderId = this.Order.Id;
            viewModel.OpenEmailPage();
            windowService.ShowSingleMessageWindow(viewModel);
        }
        public void EditGlassesOrder()
        {
            this.Order.Customer = null;
            if (this.Order.EyeGlassFrame.Id == 0)
                this.Order.EyeGlassFrame_Id = null;
            else
                this.Order.EyeGlassFrame_Id = this.Order.EyeGlassFrame.Id;

            if (this.Order.GlassType.Id == 0)
                this.Order.GlassType_Id = null;
            else
                this.Order.GlassType_Id = this.Order.GlassType.Id;

            if (this.Order.Doctor.Id == 0)
                this.Order.Doctor_Id = null;
            else
                this.Order.Doctor_Id = this.Order.Doctor.Id;

            this.Order.EyeGlassFrame = null;
            this.Order.Doctor = null;
            this.Order.GlassType = null;
            try
            {
                this.Uow.OrderRepository.Update(this.Order);
                this.Uow.Save();
                if(this.Order.ProcessingState == "Abgeholt" && this.Order.EyeGlassFrame_Id != null)
                {
                    this.Order.EyeGlassFrame.State = "Verkauft";
                    this.Uow.EyeGlassFrameRepository.Update(this.Order.EyeGlassFrame);
                    this.Uow.Save();
                }
                //for debugging
                //foreach (var prop in typeof(Order).GetProperties())
                //{
                //    Console.WriteLine(prop.Name + ": " + this.Order.GetType().GetProperty(prop.Name).GetValue(this.Order, null));
                //}
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
        private void Calculate()
        {
            decimal sum1 = Order.GlassPriceLeft + Order.GlassPriceRight + Convert.ToDecimal(Order.OthersPrice) + Convert.ToDecimal(Order.EyeGlassFrame?.SalePrice);
            decimal sum2 = sum1 - Convert.ToDecimal(Order.InsurancePrice) + Convert.ToDecimal(Order.PatientsContribution) - Convert.ToDecimal(Order.Discount);
            Order.GrossPrice = sum2;
            Order.BettermentTax = Order.GrossPrice - (sum2 / Convert.ToDecimal(1.2));
            RaisePropertyChanged(() => this.Order);
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
            var egf = this.Uow.EyeGlassFrameRepository.Get(filter: e => e.State != "Verkauft").ToList();
            egf.Insert(0, new EyeGlassFrame() { ModelDescription = "Bitte wählen..." });
            this.EyeGlassFrames = egf;
            RaisePropertyChanged(() => this.EyeGlassFrames);

            this.Order.EyeGlassFrame = EyeGlassFrames[0];
            RaisePropertyChanged(() => this.Order);
        }
        private void SetFields()
        {
            FillDoctors();
            FillGlassTypes();
            FillEyeGlassFrames();
            this.RaisePropertyChanged(() => this.Order);
        }
    }
}
