using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class MultipleEmailViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<CustomRecipient> Recipients { get; set; }
        public ICommand Send { get; set; }

        public MultipleEmailViewModel(IUnitOfWork uow)
        {
            this.Uow = uow;
            Send = new RelayCommand(SendMessage);
            this.Recipients = new List<CustomRecipient>();
        }
        public void Init()
        {
            this.Message = String.Empty;
            this.Subject = String.Empty;
            this.RaisePropertyChanged(() => this.Subject);
            this.RaisePropertyChanged(() => this.Message);
            this.Recipients = new List<CustomRecipient>();
        }
        private async void SendMessage()
        {
            try
            {

                var customers = this.Uow.CustomerRepository.Get(filter: c => c.NewsLetter && c.Deleted == false);
                var result = MessageBox.Show("Wollen Sie diese Nachricht wirklich an " + customers.Length + " Kunden senden?", "Wollen Sie die Nachricht abschicken?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
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
                        foreach (var item in customers)
                        {
                            try
                            {
                                var message = new MailMessage();
                                message.To.Add(new MailAddress(item.Email));
                                message.From = new MailAddress("infodienst.augenoptikaigner@gmail.com");
                                message.Subject = this.Subject;
                                message.Body = this.Message;
                                this.Recipients.Add(new CustomRecipient() { Customer = item, Address = item.Email });

                                await smtp.SendMailAsync(message);
                            }

                            catch (Exception ex)
                            {
                                if(ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException)
                                    MessageBox.Show(String.Format("Ein Fehler beim Kunden '{0}' ist aufgetreten! Die E-Mail konnte nicht an diesen Kunden gesendet werden! {1} {2}", item.LastName, Environment.NewLine, ex.Message), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                else
                                    throw;
                            }
                        }
                    }
                    SaveToRepository();
                    MessageBox.Show("Der Sendevorgang wurde beendet, bitte überprüfe Sie Ihren E-Mail-Eingang um sicherzustellen, dass alle E-Mails korrekt versendet wurden.", "Versendet", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("Beim Versenden der Emails ist ein Fehler aufgetreten! {0} {1}", Environment.NewLine, ex.Message), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Init();
        }
        private void SaveToRepository()
        {
            var m = new CustomMessage();
            m.Date = DateTime.Now;
            m.MessageText = this.Message;
            m.MessageType = OpticiatnMgr.Core.Entities.MessageType.EMail;
            m.Recipients = this.Recipients;
            m.Subject = this.Subject;
            this.Uow.MessageRepository.Insert(m);
            this.Uow.Save();
        }
    }
}