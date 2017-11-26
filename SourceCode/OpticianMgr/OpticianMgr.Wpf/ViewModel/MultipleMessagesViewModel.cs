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
    public class MultipleMessagesViewModel : ViewModelBase
    {
        private MultipleEMailPage MultipleEmailPage { get; set; }
        private MultipleSMSPage MultipleSMSPage { get; set; }
        public ICommand EMail { get; set; }
        public ICommand SMS { get; set; }
        public object Page { get; set; }

        public MultipleMessagesViewModel()
        {
            this.MultipleEmailPage = new MultipleEMailPage();
            this.MultipleSMSPage = new MultipleSMSPage();
            SMS = new RelayCommand(OpenSMSPage);
            EMail = new RelayCommand(OpenEmailPage);
        }

        //TODO Is this MVVM?
        public void OpenEmailPage()
        {
            MultipleEmailViewModel viewModel = ViewModelLocator.MultipleEmailViewModel;
            this.MultipleEmailPage.DataContext = viewModel;
            this.Page = this.MultipleEmailPage;
            this.RaisePropertyChanged(() => this.Page);
        }
        public void OpenSMSPage()
        {
            MultipleSMSViewModel viewModel = ViewModelLocator.MultipleSMSViewModel;
            this.MultipleSMSPage.DataContext = viewModel;
            this.Page = this.MultipleSMSPage;
            this.RaisePropertyChanged(() => this.Page);
        }
    }
}