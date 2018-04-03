using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.Pages;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;

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
    public class SingleMessageViewModel : ViewModelBase, IRequestClose
    {
        private SingleEMailPage SingleEMailPage { get; set; }
        private SingleSMSPage SingleSMSPage { get; set; }
        public ICommand EMail { get; set; }
        public ICommand SMS { get; set; }
        public object Page { get; set; }
        public int OrderId { get; set; }
        public event EventHandler<EventArgs> CloseRequested;

        public SingleMessageViewModel()
        {
            this.SingleEMailPage = new SingleEMailPage();
            this.SingleSMSPage = new SingleSMSPage();
            SMS = new RelayCommand(OpenSMSPage);
            EMail = new RelayCommand(OpenEmailPage);

            EventHandler<EventArgs> closeHandler = null;
            closeHandler = (sender, e) =>
            {
                this.CloseRequested?.Invoke(this,null);
            };
            ViewModelLocator.SingleEmailViewModel.CloseRequested += closeHandler;
            ViewModelLocator.SingleSMSViewModel.CloseRequested += closeHandler;
        }

        //TODO Is this MVVM?
        public void OpenEmailPage()
        {
            SingleEmailViewModel viewModel = ViewModelLocator.SingleEmailViewModel;
            viewModel.Init(this.OrderId);
            this.SingleEMailPage.DataContext = viewModel;
            this.Page = this.SingleEMailPage;
            this.RaisePropertyChanged(() => this.Page);
        }
        public void OpenSMSPage()
        {
            SingleSMSViewModel viewModel = ViewModelLocator.SingleSMSViewModel;
            viewModel.Init(this.OrderId);
            this.SingleSMSPage.DataContext = viewModel;
            this.Page = this.SingleSMSPage;
            this.RaisePropertyChanged(() => this.Page);
        }
    }
}