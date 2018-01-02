using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class EditGlasstypesViewModel : ViewModelBase, IRequestClose
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenGlasstype { get; set; }
        public ICommand AddGlasstype { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<Glasstype> Glasstypes { get; set; }
        public ICollectionView GlasstypesView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;

        public event EventHandler<EventArgs> CloseRequested;

        public string TranslatedSortProperty { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public string SortProperty { get; set; }
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Glasstype).GetProperties().Select(g => g.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp"); //Shouldnt be able to filter by timestamp
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        public EditGlasstypesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.SortProperty = "Id";
            this.FilterProperty = "Glastypbezeichnung";
            this.Glasstypes = GetAllGlasstypes();
            this.GlasstypesView = CollectionViewSource.GetDefaultView(Glasstypes);
            OpenGlasstype = new RelayCommand(OpenG);
            AddGlasstype = new RelayCommand(AddG);
            FilterAndSort = new RelayCommand(FilterAndSortGlasstypes);
            DeleteFilter = new RelayCommand(DeleteF);
        }
        public void DeleteF()
        {
            this.FilterText = "";
            FilterAndSortGlasstypes();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Glasstypes = GetAllGlasstypes();
            this.RaisePropertyChanged(() => this.Glasstypes);
            this.GlasstypesView = CollectionViewSource.GetDefaultView(Glasstypes);
            FilterAndSortGlasstypes();
            this.RaisePropertyChanged(() => this.GlasstypesView);
        }
        public void FilterAndSortGlasstypes()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.TranslatedFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            this.TranslatedSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortProperty).Key?.ToString();
            Filter();
            Sort();
        }
        public void Sort()
        {
            try
            {
                this.GlasstypesView.SortDescriptions.Clear();
                if (typeof(Glasstype).GetProperty(TranslatedSortProperty) != null)
                    this.GlasstypesView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.GlasstypesView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.GlasstypesView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool Contains(object g)
        {
            Glasstype glasstype = g as Glasstype;
            if (typeof(Glasstype).GetProperty(TranslatedFilterProperty) != null)
            {
                return glasstype.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(glasstype, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        public void OpenG()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                GlassTypeDetailsViewModel viewModel = ViewModelLocator.GlassTypeDetailsViewModel;
                viewModel.InitGlassType(((Glasstype)this.Selected).Id);
                EventHandler<EventArgs> refreshGlasstypesHandler = null;
                refreshGlasstypesHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshGlasstypesHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshGlasstypesHandler;
                windowService.ShowGlasstypeDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst einen Glastyp aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void AddG()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddGlasstypeViewModel viewModel = ViewModelLocator.AddGlasstypeViewModel;
            EventHandler<EventArgs> refreshGlasstypesHandler = null;
            refreshGlasstypesHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshGlasstypesHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshGlasstypesHandler;
            windowService.ShowAddGlasstypeWindow(viewModel);
        }
        private ObservableCollection<Glasstype> GetAllGlasstypes()
        {
            var unitOfWorkGlasstypes = this.Uow.GlassTypeRepository.Get().ToList();
            ObservableCollection<Glasstype> copiedGlasstypes = new ObservableCollection<Glasstype>();
            foreach (var item in unitOfWorkGlasstypes)
            {
                Glasstype glasstype = new Glasstype();
                GenericRepository<Glasstype>.CopyProperties(glasstype, item);
                copiedGlasstypes.Add(glasstype);
            }
            return copiedGlasstypes;
        }
    }
}
