using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticiatnMgr.Core.Contracts;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SingleSMSViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public string To { get; set; }
        public string Message { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Send { get; set; }

        public SingleSMSViewModel(IUnitOfWork uow)
        {
            this.Uow = uow;
            Cancel = new RelayCommand(CancelSendMessage);
            Send = new RelayCommand(SendMessage);
        }
        public void Init(int orderId)
        {
            var order = this.Uow.OrderRepository.GetById(orderId);
            this.To = String.IsNullOrEmpty(order.Customer.Telephone1) ? order.Customer.Telephone2 : order.Customer.Telephone1;
            switch (order.OrderType)
            {
                case "B":
                    this.Message = String.Format("Ihre Brillenbestellung ist nun abholbereit!");
                    break;
                case "K":
                    this.Message = String.Format("Ihre Kontaktlinsenbestellung ist nun abholbereit!");
                    break;
            }
            this.RaisePropertyChanged(() => this.To);
            this.RaisePropertyChanged(() => this.Message);
        }

        private void CancelSendMessage()
        {
            this.CloseRequested?.Invoke(this, null);
        }
        private void SendMessage()
        {
            MessageBox.Show("Auf Grund der SMS-Kosten, ist dieses Feature noch nicht implementiert!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            this.CloseRequested?.Invoke(this, null);
        }

    }
}