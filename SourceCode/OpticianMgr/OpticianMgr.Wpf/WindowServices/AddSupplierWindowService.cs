using OpticianMgr.Wpf.Pages;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf
{
    public class AddSupplierWindowService : IWindowService
    {
        public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            AddSupplierWindow newWindow = new AddSupplierWindow();
            EventHandler<EventArgs> closeHandler = null;
            closeHandler = (sender, e) =>
            {
                viewModel.CloseRequested -= closeHandler;
                newWindow.Close();
            };
            viewModel.CloseRequested += closeHandler;
            newWindow.DataContext = viewModel;
            newWindow.Show();
        }
    }
}
