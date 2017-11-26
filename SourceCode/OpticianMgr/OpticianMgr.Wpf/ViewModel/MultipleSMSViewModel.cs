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
    public class MultipleSMSViewModel : ViewModelBase
    {
        private IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public string Message { get; set; }
        public ICommand Send { get; set; }

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
            MessageBox.Show("Auf Grund der SMS-Kosten, ist dieses Feature noch nicht implementiert!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            this.CloseRequested?.Invoke(this, null);
        }

    }
}