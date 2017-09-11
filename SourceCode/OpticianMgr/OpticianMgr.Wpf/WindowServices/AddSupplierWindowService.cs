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
    public class AddSupplierWindowService : IWindowService
    {
        //TODO nicht für jedes Fenster einen Service erstellen
        public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : IRequestClose
        {
            if (System.Windows.Application.Current.Windows.OfType<AddSupplierWindow>().Count() == 0)
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
            else
            {
                MessageBox.Show("Es ist schon ein Fenster für das Anlegen eines neuen Lieferanten geöffnet!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
