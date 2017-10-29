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
            SimpleIoc.Default.Register<StatisticsViewModel>();
            SimpleIoc.Default.Register<EyeGlassFramesViewModel>();
            SimpleIoc.Default.Register<AddEyeGlassFrameModel>();
            SimpleIoc.Default.Register<AddCustomerViewModel>();
            SimpleIoc.Default.Register<AddTownViewModel>();
            SimpleIoc.Default.Register<AddCountryViewModel>();
            SimpleIoc.Default.Register<CustomerDetailsViewModel>();
            SimpleIoc.Default.Register<EyeGlassFramesDetailsViewModel>();
            SimpleIoc.Default.Register<SupplierDetailsViewModel>();
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
        //TODO is static okk here?
        public static AddSupplierViewModel AddSupplierViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddSupplierViewModel>(); }
        }
        public CustomerViewModel CustomerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CustomerViewModel>(); }
        }
        public StatisticsViewModel StatisticsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<StatisticsViewModel>(); }
        }
        public EyeGlassFramesViewModel EyeGlassFramesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EyeGlassFramesViewModel>(); }
        }
        //TODO
        public static AddEyeGlassFrameModel AddEyeGlassFrameModel
        {
            get { return ServiceLocator.Current.GetInstance<AddEyeGlassFrameModel>(); }
        }
        //TODO
        public static AddCustomerViewModel AddCustomerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddCustomerViewModel>(); }
        }
        public static AddTownViewModel AddTownViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddTownViewModel>(); }
        }
        public static AddCountryViewModel AddCountryViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddCountryViewModel>(); }
        }
        public static CustomerDetailsViewModel CustomerDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CustomerDetailsViewModel>(); }
        }
        public static EyeGlassFramesDetailsViewModel EyeGlassFramesDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EyeGlassFramesDetailsViewModel>(); }
        }
        public static SupplierDetailsViewModel SupplierDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SupplierDetailsViewModel>(); }
        }
        public static void Cleanup()
        {
                        // TODO Clear the ViewModels
        }
    }
}