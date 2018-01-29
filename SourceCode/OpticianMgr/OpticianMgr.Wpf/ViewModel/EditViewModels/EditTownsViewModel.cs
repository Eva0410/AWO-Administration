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
    public class EditTownsViewModel : ViewModelBase, IRequestClose
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenTown { get; set; }
        public ICommand AddTown { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<Town> Towns { get; set; }
        public ICollectionView TownsView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;

        public event EventHandler<EventArgs> CloseRequested;

        public string TranslatedSortProperty { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public string SortProperty { get; set; }
        public ObservableCollection<String> PropertiesList { get; }
        public EditTownsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.PropertiesList = GetAllProperties();
            this.SortProperty = "Id";
            this.FilterProperty = "Ortsname";
            this.Towns = GetAllTowns();
            this.TownsView = CollectionViewSource.GetDefaultView(Towns);
            OpenTown = new RelayCommand(OpenT);
            AddTown = new RelayCommand(AddT);
            FilterAndSort = new RelayCommand(FilterAndSortTowns);
            DeleteFilter = new RelayCommand(DeleteF);
        }
        private ObservableCollection<string> GetAllProperties()
        {
            ObservableCollection<string> props = new ObservableCollection<string>(typeof(Town).GetProperties().Select(t => t.Name).ToList());
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
        public void DeleteF()
        {
            this.FilterText = "";
            FilterAndSortTowns();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Towns = GetAllTowns();
            this.RaisePropertyChanged(() => this.Towns);
            this.TownsView = CollectionViewSource.GetDefaultView(Towns);
            FilterAndSortTowns();
            this.RaisePropertyChanged(() => this.TownsView);
        }
        public void FilterAndSortTowns()
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
                this.TownsView.SortDescriptions.Clear();
                if (typeof(Town).GetProperty(TranslatedSortProperty) != null)
                    this.TownsView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.TownsView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.TownsView.Filter = null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private bool Contains(object t)
        {
            Town town = t as Town;
            if (typeof(Town).GetProperty(TranslatedFilterProperty) != null)
            {
                return town.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(town, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        public void OpenT()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                TownDetailsViewModel viewModel = ViewModelLocator.TownDetailsViewModel;
                viewModel.InitTown(((Town)this.Selected).Id);
                EventHandler<EventArgs> refreshTownsHandler = null;
                refreshTownsHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshTownsHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshTownsHandler;
                windowService.ShowTownDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst einen Ort aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void AddT()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddTownViewModel viewModel = ViewModelLocator.AddTownViewModel;
            EventHandler<EventArgs> refreshTownsHandler = null;
            refreshTownsHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshTownsHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshTownsHandler;
            windowService.ShowAddTownWindow(viewModel);
        }
        private ObservableCollection<Town> GetAllTowns()
        {
            var unitOfWorkTowns = this.Uow.TownRepository.Get().ToList();
            ObservableCollection<Town> copiedTowns = new ObservableCollection<Town>();
            foreach (var item in unitOfWorkTowns)
            {
                Town town = new Town();
                GenericRepository<Town>.CopyProperties(town, item);
                copiedTowns.Add(town);
            }
            return copiedTowns;
        }
    }
}
