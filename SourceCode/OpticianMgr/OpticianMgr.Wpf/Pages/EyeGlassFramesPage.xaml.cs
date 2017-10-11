using OpticianMgr.Persistence;
using OpticianMgr.Wpf.ViewModel;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpticianMgr.Wpf
{
    /// <summary>
    /// Interaction logic for SupplierPage.xaml
    /// </summary>
    public partial class EyeGlassFramesPage : Page
    {
        public EyeGlassFramesPage()
        {
            InitializeComponent();
            //TODO fix binding
            dgState.ItemsSource = EyeGlassFramesViewModel.States;
            using (UnitOfWork uow = new UnitOfWork())
            {
                //TODO Liste von Suppliern und nicht von Namen!
                var sups = uow.SupplierRepository.Get(orderBy: o => o.OrderBy(s => s.Name)).ToList();
                sups.Insert(0, new Supplier() { Name = " " });
                dgSup.ItemsSource = sups;
            }
        }
    }
}
