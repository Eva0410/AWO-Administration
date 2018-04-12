using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBird;
using MessageBird.Exceptions;
using MessageBird.Net.ProxyConfigurationInjector;
using MessageBird.Objects;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
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
        public Order Order { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Send { get; set; }
        const string YourAccessKey = "SBQA6BUHfbBlpRsgCGMJ4olfe"; // message bird access key
        //SBQA6BUHfbBlpRsgCGMJ4olfe für echte sms
        //vrh38QWVVXeW0D1Ma3ENhmd3a für test sms

        public SingleSMSViewModel(IUnitOfWork uow)
        {
            this.Uow = uow;
            Cancel = new RelayCommand(CancelSendMessage);
            Send = new RelayCommand(SendMessage);
        }
        public void Init(int orderId)
        {
            var order = this.Uow.OrderRepository.GetById(orderId);
            this.Order = order;
            this.To = String.IsNullOrEmpty(order.Customer.Telephone1) ? order.Customer.Telephone2 : order.Customer.Telephone1;
            switch (order.OrderType)
            {
                case "B":
                    this.Message = String.Format(Properties.Settings.Default.SingleSMSTextGlassesOrder, Environment.NewLine, order.Customer.Title, order.Customer.FirstName, order.Customer.LastName);
                    break;
                case "K":
                    this.Message = String.Format(Properties.Settings.Default.SingleSMSTextContactLensOrder, Environment.NewLine, order.Customer.Title, order.Customer.FirstName, order.Customer.LastName);
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
            IProxyConfigurationInjector proxyConfigurationInjector = null; // for no web proxies, or web proxies not requiring authentication

            Client client = Client.CreateDefault(YourAccessKey, proxyConfigurationInjector);

            try
            {
                MessageBird.Objects.Message message = client.SendMessage(Properties.Settings.Default.SmsSenderText, this.Message, new[] { Convert.ToInt64(this.To) });
                SaveToRepository();
                MessageBox.Show("Nachricht gesendet!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                Console.WriteLine("{0}", message);
            }
            catch (ErrorException e)
            {
                if (e.HasErrors)
                {
                    foreach (Error error in e.Errors)
                    {
                        Console.WriteLine("code: {0} description: '{1}' parameter: '{2}'", error.Code, error.Description, error.Parameter);
                        MessageBox.Show(String.Format("Tipp: Ist die Telefonnummer korrekt? Sie muss im '0043xxxxxxxxxx' angegeben werden! {3} code: {0} description: '{1}' parameter: '{2}'", error.Code, error.Description, error.Parameter, Environment.NewLine), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (e.HasReason)
                {
                    Console.WriteLine(e.Reason);
                    MessageBox.Show(e.Reason, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Ein Fehler ist aufgetreten!" + Environment.NewLine + e.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(e.ToString());
            }
            this.CloseRequested?.Invoke(this,null);
        }
        private void SaveToRepository()
        {
            var m = new CustomMessage();
            m.Date = DateTime.Now;
            m.MessageText = this.Message;
            m.MessageType = OpticiatnMgr.Core.Entities.MessageType.SMS;
            m.Recipients = new List<CustomRecipient>();
            m.Recipients.Add(new CustomRecipient() { Customer = this.Order.Customer, Address = this.To });
            m.Order_Id = this.Order.Id;
            this.Uow.MessageRepository.Insert(m);
            this.Uow.Save();
        }
    }
}