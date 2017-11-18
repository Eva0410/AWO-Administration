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
    public class ContactLensOrderDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;

        public Order Order { get; set; }
        public List<Doctor> Doctors { get; set; }
        public string[] ProcessingStates { get; set; }
        public string[] PaymentStates { get; set; }
        public List<ContactLensType> ContactLensTypes { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand AddDoctor { get; set; }
        public ICommand Calc { get; set; }
        public ICommand Delete { get; set; }
        public ContactLensOrderDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditContactLensOrder);
            Submit = new RelayCommand(EditContactLensOrder);
            AddDoctor = new RelayCommand(AddD);
            Calc = new RelayCommand(Calculate);
            Delete = new RelayCommand(DeleteCLO);
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
            if (item.ContactLensType_Id != null)
            {
                ContactLensType contactLensType = new ContactLensType();
                GenericRepository<ContactLensType>.CopyProperties(contactLensType, this.Uow.ContactLensTypeRepository.GetById(item.ContactLensType_Id));
                order.ContactLensType = contactLensType;
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
            this.Order.ContactLensType = this.Order.ContactLensType_Id== null ? ContactLensTypes[0] : ContactLensTypes.Where(c => c.Id == this.Order.ContactLensType_Id).FirstOrDefault();
        }
        public void DeleteCLO()
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
        public void CancelEditContactLensOrder()
        {
            this.CloseRequested?.Invoke(this, null);
            this.SetFields();
        }
        public void EditContactLensOrder()
        {
            this.Order.Customer = null;

            if (this.Order.ContactLensType.Id == 0)
                this.Order.ContactLensType_Id = null;
            else
                this.Order.ContactLensType_Id = this.Order.ContactLensType.Id;

            if (this.Order.Doctor.Id == 0)
                this.Order.Doctor_Id = null;
            else
                this.Order.Doctor_Id = this.Order.Doctor.Id;

            this.Order.Doctor = null;
            this.Order.ContactLensType = null;
            try
            {
                this.Uow.OrderRepository.Update(this.Order);
                this.Uow.Save();
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
            decimal sum1 = Order.GlassPriceLeft + Order.GlassPriceRight + Convert.ToDecimal(Order.OthersPrice);
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
        private void FillContactLensTypes()
        {
            var contactLensTypes = this.Uow.ContactLensTypeRepository.Get(orderBy: o => o.OrderBy(c => c.ContactLensTypeDescription)).ToList();
            contactLensTypes.Insert(0, new ContactLensType() { ContactLensTypeDescription = "Bitte wählen..." });
            this.ContactLensTypes = contactLensTypes;
            RaisePropertyChanged(() => this.ContactLensTypes);

            this.Order.ContactLensType = ContactLensTypes[0];
            RaisePropertyChanged(() => this.Order);
        }
        private void SetFields()
        {
            FillDoctors();
            FillContactLensTypes();
            this.RaisePropertyChanged(() => this.Order);
        }
    }
}
