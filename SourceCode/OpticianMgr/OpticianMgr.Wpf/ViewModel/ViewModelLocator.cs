/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:OpticianMgr.Wpf"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using Microsoft.Extensions.Configuration;

namespace OpticianMgr.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            SimpleIoc.Default.Reset();
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<UnitOfWork>(() =>
            {
                return new UnitOfWork();
            });
            SimpleIoc.Default.Register<IUnitOfWork, UnitOfWork>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SupplierViewModel>();
            SimpleIoc.Default.Register<AddSupplierViewModel>();
            SimpleIoc.Default.Register<CustomerViewModel>();
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SupplierViewModel SupplierViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SupplierViewModel>(); }
        }
        //TODO is static okk here
        public static AddSupplierViewModel AddSupplierViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddSupplierViewModel>(); }
        }
        public CustomerViewModel CustomerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CustomerViewModel>(); }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}