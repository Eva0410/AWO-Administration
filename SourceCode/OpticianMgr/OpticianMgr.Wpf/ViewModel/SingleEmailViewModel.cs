using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.Pages;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Windows;
using System.Net.Mail;
using System.Net;

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
    public class SingleEmailViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Send { get; set; }

        public SingleEmailViewModel(IUnitOfWork uow)
        {
            this.Uow = uow;
            Cancel = new RelayCommand(CancelSendMessage);
            Send = new RelayCommand(SendMessage);
        }
        public void Init(int orderId)
        {
            var order = this.Uow.OrderRepository.GetById(orderId);
            this.To = order.Customer.Email;
            switch (order.OrderType)
            {
                case "B":
                    this.Subject = "Ihre Brillenbestellung von Augenoptik Aigner";
                    this.Message = String.Format("Sehr geehrte/r Frau/Herr {0} {1}, {2} {2} Ihre Brillenbestellung ist nun abholbereit! {2} {2} Dies ist eine automatisch versendete Nachricht, bitte antworten Sie nicht auf diese EMail. {2} {2} Mit freundlichen Grüßen {2} Augenoptik Aigner", order.Customer.Title, order.Customer.LastName, Environment.NewLine);
                    break;
                case "K":
                    this.Subject = "Ihre Kontaktlinsenbestellung von Augenoptik Aigner";
                    this.Message = String.Format("Sehr geehrte/r Frau/Herr {0} {1}, {2} {2} Ihre Kontaktlinsenbestellung ist nun abholbereit! {2} {2} Dies ist eine automatisch versendete Nachricht, bitte antworten Sie nicht auf diese EMail. {2} {2} Mit freundlichen Grüßen {2} Augenoptik Aigner", order.Customer.Title, order.Customer.LastName, Environment.NewLine);
                    break;
            }
            this.RaisePropertyChanged(() => this.Subject);
            this.RaisePropertyChanged(() => this.To);
            this.RaisePropertyChanged(() => this.Message);
        }

        private void CancelSendMessage()
        {
            this.CloseRequested?.Invoke(this, null);
        }
        private void SendMessage()
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(this.To));
            message.From = new MailAddress("infodienst.augenoptikaigner@gmail.com");
            message.Subject = this.Subject;
            message.Body = this.Message;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "infodienst.augenoptikaigner@gmail.com",
                    Password = "7gnRwN4U"
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.SendMailAsync(message);
            }
            this.CloseRequested.Invoke(this, null);
        }
    }
}