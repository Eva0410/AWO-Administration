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
            SimpleIoc.Default.Register<IUnitOfWork>(() => new UnitOfWork());
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
            SimpleIoc.Default.Register<OrdersViewModel>();
            SimpleIoc.Default.Register<AddGlassesOrderViewModel>();
            SimpleIoc.Default.Register<AddContactLensesOrderViewModel>();
            SimpleIoc.Default.Register<AddDoctorViewModel>();
            SimpleIoc.Default.Register<GlassesOrderDetailsViewModel>();
            SimpleIoc.Default.Register<ContactLensOrderDetailsViewModel>();
            SimpleIoc.Default.Register<SingleEmailViewModel>();
            SimpleIoc.Default.Register<SingleMessageViewModel>();
            SimpleIoc.Default.Register<SingleSMSViewModel>();
            SimpleIoc.Default.Register<MultipleMessagesViewModel>();
            SimpleIoc.Default.Register<MultipleEmailViewModel>();
            SimpleIoc.Default.Register<MultipleSMSViewModel>();
            SimpleIoc.Default.Register<SentMessagesViewModel>();
            SimpleIoc.Default.Register<EditTownsViewModel>();
            SimpleIoc.Default.Register<TownDetailsViewModel>();
            SimpleIoc.Default.Register<EditCountriesViewModel>();
            SimpleIoc.Default.Register<CountryDetailsViewModel>();
            SimpleIoc.Default.Register<AddGlasstypeViewModel>();
            SimpleIoc.Default.Register<EditGlasstypesViewModel>();
            SimpleIoc.Default.Register<GlassTypeDetailsViewModel>();
            SimpleIoc.Default.Register<AddContactLensTypeViewModel>();
            SimpleIoc.Default.Register<EditContactLensTypesViewModel>();
            SimpleIoc.Default.Register<ContactLensTypeDetailsViewModel>();
            SimpleIoc.Default.Register<EditStaticStringsModel>();
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
        public OrdersViewModel OrdersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<OrdersViewModel>(); }
        }
        public static AddGlassesOrderViewModel AddGlassesOrderViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddGlassesOrderViewModel>(); }
        }
        public static AddContactLensesOrderViewModel AddContactLensesOrderViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddContactLensesOrderViewModel>(); }
        }
        public static AddDoctorViewModel AddDoctorViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddDoctorViewModel>(); }
        }
        public static GlassesOrderDetailsViewModel GlassesOrderDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GlassesOrderDetailsViewModel>(); }
        }
        public static ContactLensOrderDetailsViewModel ContactLensOrderDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ContactLensOrderDetailsViewModel>(); }
        }
        public static SingleMessageViewModel SingleMessageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SingleMessageViewModel>(); }
        }
        public static SingleEmailViewModel SingleEmailViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SingleEmailViewModel>(); }
        }
        public static SingleSMSViewModel SingleSMSViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SingleSMSViewModel>(); }
        }
        public static MultipleMessagesViewModel MultipleMessagesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MultipleMessagesViewModel>(); }
        }
        public static MultipleEmailViewModel MultipleEmailViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MultipleEmailViewModel>(); }
        }
        public static MultipleSMSViewModel MultipleSMSViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MultipleSMSViewModel>(); }
        }
        public static SentMessagesViewModel SentMessagesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SentMessagesViewModel>(); }
        }
        public static EditTownsViewModel EditTownsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditTownsViewModel>(); }
        }
        public static TownDetailsViewModel TownDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<TownDetailsViewModel>(); }
        }
        public static EditCountriesViewModel EditCountriesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCountriesViewModel>(); }
        }
        public static CountryDetailsViewModel CountryDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CountryDetailsViewModel>(); }
        }
        public static AddGlasstypeViewModel AddGlasstypeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddGlasstypeViewModel>(); }
        }
        public static EditGlasstypesViewModel EditGlasstypesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditGlasstypesViewModel>(); }
        }
        public static GlassTypeDetailsViewModel GlassTypeDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GlassTypeDetailsViewModel>(); }
        }
        public static AddContactLensTypeViewModel AddContactLensTypeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddContactLensTypeViewModel>(); }
        }
        public static ContactLensTypeDetailsViewModel ContactLensTypeDetailsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ContactLensTypeDetailsViewModel>(); }
        }
        public static EditContactLensTypesViewModel EditContactLensTypesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditContactLensTypesViewModel>(); }
        }
        public static EditStaticStringsModel EditStaticStringsModel
        {
            get { return ServiceLocator.Current.GetInstance<EditStaticStringsModel>(); }
        }

        public static void Cleanup()
        {
                        // TODO Clear the ViewModels
        }
    }
}