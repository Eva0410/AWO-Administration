﻿using OpticianMgr.Wpf.Pages;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpticianMgr.Wpf
{
    public class WindowService : IWindowService
    {
        //TODO nicht für jedes Fenster einen Service erstellen
        public void ShowAddSupplierWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddSupplierWindow newWindow = new AddSupplierWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddEyeGlassesFrameWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddEyeGlassesFrameWindow newWindow = new AddEyeGlassesFrameWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddCustomerWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddCustomerWindow newWindow = new AddCustomerWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddTownWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddTownWindow newWindow = new AddTownWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddCountryWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddCountryWindow newWindow = new AddCountryWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowCustomerDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            CustomerDetailsWindow newWindow = new CustomerDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEyeGlassFrameDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EyeGlassFrameDetailsWindow newWindow = new EyeGlassFrameDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowSupplierDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            SupplierDetailsWindow newWindow = new SupplierDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddGlassesOrderWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddGlassesOrderWindow newWindow = new AddGlassesOrderWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddContactLensOrderWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddContactLensesOrderWindow newWindow = new AddContactLensesOrderWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddDoctorWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddDoctorWindow newWindow = new AddDoctorWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowGlassesOrderDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            GlassesOrderDetailsWindow newWindow = new GlassesOrderDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowContactLensOrderDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            ContactLensOrderDetailsWindow newWindow = new ContactLensOrderDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowSingleMessageWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            SingleMessageWindow newWindow = new SingleMessageWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowSentMessagesWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            SentMessagesWindow newWindow = new SentMessagesWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEditTownsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EditTownWindow newWindow = new EditTownWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowTownDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            TownDetailsWindow newWindow = new TownDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEditCountriesWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EditCountriesWindow newWindow = new EditCountriesWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowCountryDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            CountryDetailsWindow newWindow = new CountryDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddGlasstypeWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddGlasstypeWindow newWindow = new AddGlasstypeWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEditGlasstypesWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EditGlasstypesWindow newWindow = new EditGlasstypesWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowGlasstypeDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            GlasstypeDetailsWindow newWindow = new GlasstypeDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowAddContactLensTypeWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddContactLensTypeWindow newWindow = new AddContactLensTypeWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowContactLensTypeDetailsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            ContactLensTypeDetailsWindow newWindow = new ContactLensTypeDetailsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEditContactLensTypesWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EditContactLensTypesWindow newWindow = new EditContactLensTypesWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowEditStaticStringsWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            EditStaticStringsWindow newWindow = new EditStaticStringsWindow();
            this.ShowWindow(viewModel, newWindow);
        }
        public void ShowWindow<TViewModel>(TViewModel viewModel, Window newWindow) where TViewModel : IRequestClose
        {
            EventHandler<EventArgs> closeHandler = null;
            closeHandler = (sender, e) =>
            {
                viewModel.CloseRequested -= closeHandler;
                newWindow.Close();
            };
            viewModel.CloseRequested += closeHandler;
            newWindow.DataContext = viewModel;
            newWindow.ShowDialog();
        }
    }
}
