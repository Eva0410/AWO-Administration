﻿using GalaSoft.MvvmLight;
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
    public class AddContactLensesOrderViewModel : ViewModelBase, IRequestClose
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
        public AddContactLensesOrderViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelAddContactLensOrder);
            Submit = new RelayCommand(AddContactLensOrder);
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
        public void CancelAddContactLensOrder()
        {
            this.CloseRequested?.Invoke(this, null);
            this.InitFields();
        }
        public void AddContactLensOrder()
        {
            this.Order.Customer = null;
            this.Order.OrderType = "K";
            if (this.Order.ContactLensType.Id == 0)
                this.Order.ContactLensType = null;
            if (this.Order.Doctor.Id == 0)
                this.Order.Doctor = null;
            try
            {
                this.Uow.OrderRepository.Insert(this.Order);
                this.Uow.Save();
                //for debugging
                foreach (var prop in typeof(Order).GetProperties())
                {
                    Console.WriteLine(prop.Name + ": " + this.Order.GetType().GetProperty(prop.Name).GetValue(this.Order, null));
                }
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
        private void Calculate()
        {
            decimal sum1 = Order.GlassPriceLeft + Order.GlassPriceRight + Convert.ToDecimal(Order.OthersPrice);
            decimal sum2 = sum1 - Convert.ToDecimal(Order.InsurancePrice) + Convert.ToDecimal(Order.PatientsContribution) - Convert.ToDecimal(Order.Discount);
            Order.GrossPrice = sum2;
            Order.BettermentTax = Order.GrossPrice - (sum2 / Convert.ToDecimal(1.2));
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
            this.Order.PaymentState = this.PaymentStates[0];
            this.Order.ProcessingState = this.ProcessingStates[0];
            this.Order.GlassPriceLeft = 0;
            this.Order.GlassPriceRight = 0;
            FillDoctors();
            FillContactLensTypes();
            this.Order.PaymentDate = null;
            this.Order.OrderDate = null;
            this.RaisePropertyChanged(() => this.Order);
        }
    }
}
