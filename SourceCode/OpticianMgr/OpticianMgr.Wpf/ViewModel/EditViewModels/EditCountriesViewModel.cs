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
    public class EditCountriesViewModel : ViewModelBase, IRequestClose
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenCountry { get; set; }
        public ICommand AddCountry { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<Country> Countries { get; set; }
        public ICollectionView CountriesView { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;

        public event EventHandler<EventArgs> CloseRequested;

        public string TranslatedSortProperty { get; set; }
        public string FilterProperty { get; set; }
        public string TranslatedFilterProperty { get; set; }
        public string FilterText { get; set; }
        public string SortProperty { get; set; }
        public ObservableCollection<String> PropertiesList { get; }
        public EditCountriesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.PropertiesList = GetAllProperties();
            this.SortProperty = "Id";
            this.FilterProperty = "Landname";
            this.Countries = GetAllCountries();
            this.CountriesView = CollectionViewSource.GetDefaultView(Countries);
            OpenCountry = new RelayCommand(OpenC);
            AddCountry = new RelayCommand(AddC);
            FilterAndSort = new RelayCommand(FilterAndSortCountries);
            DeleteFilter = new RelayCommand(DeleteF);
        }
        private ObservableCollection<string> GetAllProperties()
        {
            ObservableCollection<string> props = new ObservableCollection<string>(typeof(Country).GetProperties().Select(c => c.Name).ToList());
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
            FilterAndSortCountries();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.Countries = GetAllCountries();
            this.RaisePropertyChanged(() => this.Countries);
            this.CountriesView = CollectionViewSource.GetDefaultView(Countries);
            FilterAndSortCountries();
            this.RaisePropertyChanged(() => this.CountriesView);
        }
        public void FilterAndSortCountries()
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
                this.CountriesView.SortDescriptions.Clear();
                if (typeof(Country).GetProperty(TranslatedSortProperty) != null)
                    this.CountriesView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
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
                    this.CountriesView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.CountriesView.Filter = null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private bool Contains(object c)
        {
            Country country = c as Country;
            if (typeof(Country).GetProperty(TranslatedFilterProperty) != null)
            {
                return country.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(country, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
            }
            else
            {
                MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
                return false;
            }
        }
        public void OpenC()
        {
            if (this.Selected != null)
            {
                WindowService windowService = new WindowService();
                CountryDetailsViewModel viewModel = ViewModelLocator.CountryDetailsViewModel;
                viewModel.InitCountry(((Country)this.Selected).Id);
                EventHandler<EventArgs> refreshCountriesHandler = null;
                refreshCountriesHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshCountriesHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshCountriesHandler;
                windowService.ShowCountryDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst ein Land aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void AddC()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddCountryViewModel viewModel = ViewModelLocator.AddCountryViewModel;
            EventHandler<EventArgs> refreshCountriesHandler = null;
            refreshCountriesHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshCountriesHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshCountriesHandler;
            windowService.ShowAddCountryWindow(viewModel);
        }
        private ObservableCollection<Country> GetAllCountries()
        {
            var unitOfWorkCountries = this.Uow.CountryRepository.Get().ToList();
            ObservableCollection<Country> copiedCountries = new ObservableCollection<Country>();
            foreach (var item in unitOfWorkCountries)
            {
                Country country = new Country();
                GenericRepository<Country>.CopyProperties(country, item);
                copiedCountries.Add(country);
            }
            return copiedCountries;
        }
    }
}
