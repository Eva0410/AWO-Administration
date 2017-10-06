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
