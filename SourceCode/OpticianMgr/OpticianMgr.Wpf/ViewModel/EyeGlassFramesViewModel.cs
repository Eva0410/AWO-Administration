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

namespace OpticianMgr.Wpf.ViewModel
{
    public class EyeGlassFramesViewModel : ViewModelBase
    {
        public static string[] States = { "Bestellt", "Lagernd", "Verkauft" };
        //public static ObservableCollection<string> States
        //{
        //    get { return new ObservableCollection<string>() { "Bestellt", "Lagernd", "Verkauft" }; }
        //    set { }
        //}
        public List<Supplier> Suppliers { get; set; }

        public IUnitOfWork Uow { get; set; }
        public ObservableCollection<EyeGlassFrame> EyeGlassFrames { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        private DataGridCellInfo _selectedCell;

        public DataGridCellInfo CellInfo
        {
            get { return _selectedCell; }
            set
            {
                _selectedCell = value;
                this.RaisePropertyChanged(() => this.CellInfo);
            }
        }

        //TODO wrm geht ds nd
        //TODO combo box failt


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
        public string SortProperty { get; set; }
        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                this.RaisePropertyChanged(() => this.FilterText);
            }
        }


        public ICommand FilterAndSort { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ICommand AddEyeGlassFrame { get; set; }
        public ICommand EditEyeGlassFrame { get; set; }
        RelayCommand<DataGridCellEditEndingEventArgs> CellEditEndingCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public EyeGlassFramesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.FilterProperty = "Name";
            this.SortProperty = "Id";
            this.EyeGlassFrames = GetAllEyeGlassFrames();
            FilterAndSort = new RelayCommand(FillList);
            DeleteFilter = new RelayCommand(DeleteF);
            AddEyeGlassFrame = new RelayCommand(AddE);
            EditEyeGlassFrame = new RelayCommand(EditE);
            this.EyeGlassFrames.CollectionChanged += this.OnCollectionChanged;
            CellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(args => this.RaisePropertyChanged(() => this.EyeGlassFrames));
        }
        //TODO Immer die selbe collection verwenden und nicht jedes Mal neu erstellen (clear collection)
        /// <summary>
        /// Returns a list of the eyegalssframes in the database
        /// All properties must be copied, otherwise the list would reference the unit of work data
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<EyeGlassFrame> GetAllEyeGlassFrames()
        {
            var unitOfWorkEyeGlassFrames = this.Uow.EyeGlassFrameRepository.Get().ToList();
            ObservableCollection<EyeGlassFrame> copiedEyeGlassFrames = new ObservableCollection<EyeGlassFrame>();
            copiedEyeGlassFrames.CollectionChanged += this.OnCollectionChanged;
            foreach (var item in unitOfWorkEyeGlassFrames)
            {
                EyeGlassFrame egf = new EyeGlassFrame();
                GenericRepository<EyeGlassFrame>.CopyProperties(egf, item);
                Supplier sup = new Supplier(); //Referenced supplier must be copied as well
                if (item.Supplier != null)
                {
                    GenericRepository<Supplier>.CopyProperties(sup, this.Uow.SupplierRepository.GetById(item.Supplier_Id));
                    egf.Supplier = sup;
                }
                copiedEyeGlassFrames.Add(egf);
            }
            return copiedEyeGlassFrames;
        }
        //TODO: Filter und Sort in der View machen https://msdn.microsoft.com/en-us/library/ms742542(v=vs.100).aspx, https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-c-sharp
        //TODO: extrem unperformant!!
        /// <summary>
        /// Refreshes and filters the list
        /// </summary>
        public void FillList()
        {
            //TODO
            //IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            //this.FilterProperty = this.FilterProperty == "Ort" ? "Ortsname" : this.FilterProperty; //Program filters by the name of the town and not by by town-object
            //this.FilterProperty = this.FilterProperty == "Land" ? "Landname" : this.FilterProperty;
            //this.SortProperty = this.SortProperty == "Ort" ? "Ortsname" : this.SortProperty;
            //this.SortProperty = this.SortProperty == "Land" ? "Landname" : this.SortProperty;
            //string translatedFilterProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            //string translatedSortProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.SortProperty).Key?.ToString();

            //var filteredCollection = GetFilteredList(translatedFilterProperty);
            //this.EyeGlassFrames = GetSortedList(translatedSortProperty, filteredCollection);

            this.EyeGlassFrames = new ObservableCollection<EyeGlassFrame>(GetAllEyeGlassFrames());
            this.EyeGlassFrames.CollectionChanged += OnCollectionChanged;

            var suppliers = this.Uow.SupplierRepository.Get(orderBy: o => o.OrderBy(s => s.Name)).ToList();
            suppliers.Insert(0, new Supplier() { Name = "Bitte wählen..." });
            this.Suppliers = suppliers;

            this.RaisePropertyChanged(() => this.EyeGlassFrames);
        }
        public ObservableCollection<EyeGlassFrame> GetSortedList(string translatedSortProperty, ObservableCollection<EyeGlassFrame> list)
        {
            ObservableCollection<EyeGlassFrame> sortedCollection = null;
            //if (typeof(EyeGlassFrame).GetProperty(translatedSortProperty) != null)
            //{
            //    sortedCollection = new ObservableCollection<EyeGlassFrame>(list.OrderBy(e => e.GetType().GetProperty(translatedSortProperty).GetValue(e, null)));
            //}
            //else if (typeof(Supplier).GetProperty(translatedSortProperty) != null) //Does the user sort by townname or zipcode?
            //{
            //    sortedCollection = new ObservableCollection<Supplier>(list.OrderBy(e => e.Supplier?.GetType().GetProperty(translatedSortProperty).GetValue(e.Supplier, null)));

            //}
            //else
            //{
            //    MessageBox.Show("Beim Sortieren ist ein Fehler aufgetreten");
            //}
            //sortedCollection.CollectionChanged += OnCollectionChanged;

            return sortedCollection;
        }
        public ObservableCollection<EyeGlassFrame> GetFilteredList(string translatedFilterProperty)
        {
            ObservableCollection<EyeGlassFrame> filteredCollection = null;
            //if (!String.IsNullOrEmpty(this.filterText))
            //{
            //    if (typeof(EyeGlassFrame).GetProperty(translatedFilterProperty) != null)
            //    {
            //        filteredCollection = new ObservableCollection<EyeGlassFrame>(GetAllEyeGlassFrames().Where(e => e.GetType().GetProperty(translatedFilterProperty).GetValue(e, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));
            //    }
            //    else if (typeof(Supplier).GetProperty(translatedFilterProperty) != null) //Does the user filter by townname or zipcode?
            //    {
            //        filteredCollection = new ObservableCollection<Supplier>(GetAllEyeGlassFrames().Where(s => s.Town?.GetType().GetProperty(translatedFilterProperty).GetValue(s.Town, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));

            //    }
            //    else if (typeof(Country).GetProperty(translatedFilterProperty) != null) //User filters by country
            //    {
            //        filteredCollection = new ObservableCollection<Supplier>(GetAllEyeGlassFrames().Where(s => s.Country?.GetType().GetProperty(translatedFilterProperty).GetValue(s.Country, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));
            //    }
            //    else
            //    {
            //        MessageBox.Show("Beim Filtern ist ein Fehler aufgetreten!");
            //    }

            //    filteredCollection.CollectionChanged += OnCollectionChanged;
            //}
            //else
            //    filteredCollection = GetAllEyeGlassFrames();
            return filteredCollection;
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Delete a eyeglassframe
            if (e.OldItems != null)
            {
                //OldItems will always only contain one item
                foreach (EyeGlassFrame item in e.OldItems)
                {
                    var fullItem = this.Uow.EyeGlassFrameRepository.Get(filter: eg => eg.Id == item.Id, includeProperties: "Orders").FirstOrDefault();
                    if (fullItem != null)
                    {
                        var messageBoxResult = MessageBox.Show("Wollen Sie die Brillenfassung '" + fullItem.Brand + ", " + fullItem.ModelDescription + "' wirklich löschen?", "Brillenfassung Löschen", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            this.Uow.EyeGlassFrameRepository.Delete(fullItem);
                        }

                    }
                }
                this.Uow.Save();
                this.FillList();
            }
        }
        //Delete filter
        public void DeleteF()
        {
            this.FilterText = "";
            this.FillList();
        }
        //add supplier
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
        //Edit supplier 
        public void EditE()
        {
            EyeGlassFrame newEyeGlassFrame = (EyeGlassFrame)this.CellInfo.Item;
            EyeGlassFrame oldEyeGlassFrame = this.Uow.EyeGlassFrameRepository.Get(filter: e => e.Id == newEyeGlassFrame.Id, includeProperties: "Supplier").FirstOrDefault();
            newEyeGlassFrame.Supplier_Id = newEyeGlassFrame.Supplier_Id == 0 ? null : newEyeGlassFrame.Supplier_Id;
            newEyeGlassFrame.Supplier = null;
            var messageBoxResult = MessageBox.Show(String.Format("Möchten Sie die Änderungen bei der Brillenfassung mit der Id '{0}' speichern?", newEyeGlassFrame.Id), "Brillenfassung Ändern", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Uow.EyeGlassFrameRepository.Update(newEyeGlassFrame);
                this.Uow.Save();
            }
            
            this.FillList();

        }

    }
}
