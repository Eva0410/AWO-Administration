using OpticianMgr.Wpf.Pages;
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
