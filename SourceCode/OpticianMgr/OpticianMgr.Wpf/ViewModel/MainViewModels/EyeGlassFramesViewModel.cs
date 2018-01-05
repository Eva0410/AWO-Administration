using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows;
using System.Resources;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;

namespace OpticianMgr.Wpf.ViewModel
{
    public class EyeGlassFramesViewModel : ViewModelBase
    {
        public static string[] States
        {
            get { return new string[] { "Bestellt", "Lagernd", "Verkauft" }; }
        }
        public ICollectionView EyeGlassFramesView { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public IUnitOfWork Uow { get; set; }
        public ObservableCollection<EyeGlassFrame> EyeGlassFrames { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public object Selected { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand SortShift { get; set; }
        public ICommand Initialized { get; set; }
        public SortManager SortManager { get; set; }

        //The properties of the eyeglassframes class are safed in English but need to be shown in German
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(EyeGlassFrame).GetProperties().Select(p => p.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp"); //Shouldnt be able to filter by timestamp
                props.Remove("Supplier_Id"); //Shouldnt be able to filter by supplier_id
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }

        public ICommand OpenEyeGlassFrame { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ICommand AddEyeGlassFrame { get; set; }

        public EyeGlassFramesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.FilterProperty = "Modell";
            this.EyeGlassFrames = GetAllEyeGlassFrames();
            this.EyeGlassFramesView = CollectionViewSource.GetDefaultView(EyeGlassFrames);
            FilterAndSort = new RelayCommand(FilterEyeGlassFrames);
            DeleteFilter = new RelayCommand(DeleteF);
            AddEyeGlassFrame = new RelayCommand(AddE);
            OpenEyeGlassFrame = new RelayCommand(OpenE);
            EventHandler<EventArgs> refreshEyeGlassFrame = null;

            SortCommand = new RelayCommand<RoutedEventArgs>(SortS);
            SortShift = new RelayCommand<object>(SortSh);
            Initialized = new RelayCommand<RoutedEventArgs>(Init);
            SortManager = new SortManager();

            refreshEyeGlassFrame = (sender, e) =>
            {
                this.FillList();
            };
            ViewModelLocator.AddGlassesOrderViewModel.Refresh += refreshEyeGlassFrame;
            ViewModelLocator.GlassesOrderDetailsViewModel.Refresh += refreshEyeGlassFrame;
        }

        private void Init(RoutedEventArgs p)
        {
            SortManager.Init(p);
        }
        //Click without shift key
        private void SortS(RoutedEventArgs e)
        {
            var tmp = this.EyeGlassFramesView;
            SortManager.SortNormal(e, ref tmp);
        }

        //Click with shift
        private void SortSh(object p)
        {
            var tmp = EyeGlassFramesView;
            SortManager.SortShift(p, ref tmp);
        }

        /// <summary>
        /// Returns a list of the eyegalssframes in the database
        /// All properties must be copied, otherwise the list would reference the unit of work data
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<EyeGlassFrame> GetAllEyeGlassFrames()
        {
            var unitOfWorkEyeGlassFrames = this.Uow.EyeGlassFrameRepository.Get().ToList();
            ObservableCollection<EyeGlassFrame> copiedEyeGlassFrames = new ObservableCollection<EyeGlassFrame>();
            foreach (var item in unitOfWorkEyeGlassFrames)
            {
                EyeGlassFrame egf = new EyeGlassFrame();
                GenericRepository<EyeGlassFrame>.CopyProperties(egf, item);
                if (item.Supplier_Id != null)
                {
                    Supplier supplier = new Supplier(); //Referenced supplier must be copied as well
                    GenericRepository<Supplier>.CopyProperties(supplier, this.Uow.SupplierRepository.GetById(item.Supplier_Id));
                    egf.Supplier = supplier;
                }
                copiedEyeGlassFrames.Add(egf);
            }
            return copiedEyeGlassFrames;
        }
        
        /// <summary>
        /// Refreshes and filters the list
        /// </summary>
        public void FilterEyeGlassFrames()
        {
            this.TranslatedFilterProperty = TranslateGermanToEnglish(this.FilterProperty);
            Filter();
        }
        private string TranslateGermanToEnglish(string germanName)
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            germanName = germanName == "Lieferant" ? "Name" : germanName;
            return dictionary.FirstOrDefault(e => e.Value.ToString() == germanName).Key?.ToString();
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.EyeGlassFramesView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.EyeGlassFramesView.Filter = null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private bool Contains(object f)
        {
            EyeGlassFrame frame = f as EyeGlassFrame;
            if (typeof(EyeGlassFrame).GetProperty(TranslatedFilterProperty) != null)
            {
                return frame.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(frame, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else if (typeof(Supplier).GetProperty(TranslatedFilterProperty) != null) //Does the user filter by suppliername?
            {
                return frame.Supplier?.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(frame.Supplier, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        //Delete filter
        public void DeleteF()
        {
            this.FilterText = "";
            this.FilterEyeGlassFrames();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.EyeGlassFrames = GetAllEyeGlassFrames();
            this.RaisePropertyChanged(() => this.EyeGlassFrames);
            var sort = this.EyeGlassFramesView.SortDescriptions;
            this.EyeGlassFramesView = CollectionViewSource.GetDefaultView(EyeGlassFrames);
            this.EyeGlassFramesView.SortDescriptions.Clear();
            foreach (var item in sort)
            {
                this.EyeGlassFramesView.SortDescriptions.Add(item);
            }
            FilterEyeGlassFrames();
            this.RaisePropertyChanged(() => this.EyeGlassFramesView);
        }
        //add eyeglassframe
        public void AddE()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddEyeGlassFrameModel viewModel = ViewModelLocator.AddEyeGlassFrameModel;
            EventHandler<EventArgs> refreshEyeGlassFramesHandler = null;
            refreshEyeGlassFramesHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshEyeGlassFramesHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshEyeGlassFramesHandler;
            windowService.ShowAddEyeGlassesFrameWindow(viewModel);
        }

        public void OpenE()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                EyeGlassFramesDetailsViewModel viewModel = ViewModelLocator.EyeGlassFramesDetailsViewModel;
                viewModel.InitEyeGlassFrame(((EyeGlassFrame)this.Selected).Id);
                EventHandler<EventArgs> refreshEyeGlassFramesHandler = null;
                refreshEyeGlassFramesHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshEyeGlassFramesHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshEyeGlassFramesHandler;
                windowService.ShowEyeGlassFrameDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst eine Brillenfassung aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

    }

}
