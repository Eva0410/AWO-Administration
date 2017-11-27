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
    public class MultipleSMSViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public string Message { get; set; }
        public ICommand Send { get; set; }
        const string YourAccessKey = "vrh38QWVVXeW0D1Ma3ENhmd3a"; // message bird access key
        //SBQA6BUHfbBlpRsgCGMJ4olfe f�r echte sms

        public MultipleSMSViewModel(IUnitOfWork uow)
        {
            this.Uow = uow;
            Send = new RelayCommand(SendMessage);
        }
        public void Init()
        {
            this.Message = String.Empty;
            this.RaisePropertyChanged(() => this.Message);
        }

        private void SendMessage()
        {
            IProxyConfigurationInjector proxyConfigurationInjector = null; // for no web proxies, or web proxies not requiring authentication

            Client client = Client.CreateDefault(YourAccessKey, proxyConfigurationInjector);
            //TODO test ob sms auch bei mehreren kunden gehen (derzeit in testphase, dh nur an eine telefonnummer gesendet)
            long[] numbers = GetPhoneNumbers();
            try
            {
                MessageBird.Objects.Message message = client.SendMessage("MessageBird", this.Message, numbers);
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
            this.CloseRequested?.Invoke(this, null);
        }
        private long[] GetPhoneNumbers ()
        {
            var customers = this.Uow.CustomerRepository.Get();
            long[] numbers = new long[customers.Length];
            for (int i = 0; i < customers.Length; i++)
            {
                var num = String.IsNullOrEmpty(customers[i].Telephone1) ? customers[i].Telephone2 : customers[i].Telephone1;
                try
                {
                    numbers[i] = Convert.ToInt64(num);
                }
                catch(Exception ex)
                {
                    numbers[i] = 0;
                    MessageBox.Show(String.Format("Die Telefonnummer '{0}' vom Kunden '{1}' ist keine korrekte Telefonnummer!", num, customers[i].LastName), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return numbers;
        }
    }
}