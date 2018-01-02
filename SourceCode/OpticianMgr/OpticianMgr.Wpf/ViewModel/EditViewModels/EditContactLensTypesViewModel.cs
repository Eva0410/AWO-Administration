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
    public class EditContactLensTypesViewModel : ViewModelBase, IRequestClose
    {
        private IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }

        public ICommand OpenContactLensType { get; set; }
        public ICommand AddContactLensType { get; set; }
        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ObservableCollection<ContactLensType> ContactLensTypes { get; set; }
        public ICollectionView ContactLensTypesView { get; set; }
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
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(ContactLensType).GetProperties().Select(c => c.Name).ToList());
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
        public EditContactLensTypesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.SortProperty = "Id";
            this.FilterProperty = "Kontaktlinsentypbeschreibung";
            this.ContactLensTypes = GetAllContactLensType();
            this.ContactLensTypesView = CollectionViewSource.GetDefaultView(ContactLensTypes);
            OpenContactLensType = new RelayCommand(OpenC);
            AddContactLensType = new RelayCommand(AddC);
            FilterAndSort = new RelayCommand(FilterAndSortContactLensType);
            DeleteFilter = new RelayCommand(DeleteF);
        }
        public void DeleteF()
        {
            this.FilterText = "";
            FilterAndSortContactLensType();
            this.RaisePropertyChanged(() => this.FilterText);
        }
        public void FillList()
        {
            this.ContactLensTypes = GetAllContactLensType();
            this.RaisePropertyChanged(() => this.ContactLensTypes);
            this.ContactLensTypesView = CollectionViewSource.GetDefaultView(ContactLensTypes);
            FilterAndSortContactLensType();
            this.RaisePropertyChanged(() => this.ContactLensTypesView);
        }
        public void FilterAndSortContactLensType()
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
                this.ContactLensTypesView.SortDescriptions.Clear();
                if (typeof(ContactLensType).GetProperty(TranslatedSortProperty) != null)
                    this.ContactLensTypesView.SortDescriptions.Add(new SortDescription(this.TranslatedSortProperty, ListSortDirection.Ascending));
            }
            catch (Exception e) { }
        }
        public void Filter()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.FilterText))
                {
                    this.ContactLensTypesView.Filter = new Predicate<object>(Contains);
                }
                else
                    this.ContactLensTypesView.Filter = null;

            }
            catch (Exception e) { }
        }
        private bool Contains(object c)
        {
            ContactLensType contactLensTypes = c as ContactLensType;
            if (typeof(ContactLensType).GetProperty(TranslatedFilterProperty) != null)
            {
                return contactLensTypes.GetType().GetProperty(this.TranslatedFilterProperty).GetValue(contactLensTypes, null)?.ToString().ToUpper().IndexOf(this.FilterText.ToUpper()) >= 0;
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
                ContactLensTypeDetailsViewModel viewModel = ViewModelLocator.ContactLensTypeDetailsViewModel;
                viewModel.InitContactLensType(((ContactLensType)this.Selected).Id);
                EventHandler<EventArgs> refreshContactLensTypesHandler = null;
                refreshContactLensTypesHandler = (sender, e) =>
                {
                    viewModel.Refresh -= refreshContactLensTypesHandler;
                    this.FillList();
                };
                viewModel.Refresh += refreshContactLensTypesHandler;
                windowService.ShowContactLensTypeDetailsWindow(viewModel);
            }
            else
                MessageBox.Show("Bitte wählen Sie zuerst einen Kontaktlinsentyp aus!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void AddC()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            WindowService windowService = new WindowService();
            AddContactLensTypeViewModel viewModel = ViewModelLocator.AddContactLensTypeViewModel;
            EventHandler<EventArgs> refreshContactLensTypesHandler = null;
            refreshContactLensTypesHandler = (sender, e) =>
            {
                viewModel.Refresh -= refreshContactLensTypesHandler;
                this.FillList();
            };
            viewModel.Refresh += refreshContactLensTypesHandler;
            windowService.ShowAddContactLensTypeWindow(viewModel);
        }
        private ObservableCollection<ContactLensType> GetAllContactLensType()
        {
            var unitOfWorkContactLensTypes = this.Uow.ContactLensTypeRepository.Get().ToList();
            ObservableCollection<ContactLensType> copiedContactLensTypes = new ObservableCollection<ContactLensType>();
            foreach (var item in unitOfWorkContactLensTypes)
            {
                ContactLensType contactLensType = new ContactLensType();
                GenericRepository<ContactLensType>.CopyProperties(contactLensType, item);
                copiedContactLensTypes.Add(contactLensType);
            }
            return copiedContactLensTypes;
        }
    }
}
