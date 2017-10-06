using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Windows.Controls;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class EyeGlassFramesViewModel : ViewModelBase
    {
        public static string[] States = { "Bestellt", "Lagernd", "Verkauft" };

        public IUnitOfWork Uow { get; set; }
        public ObservableCollection<EyeGlassFrame> EyeGlassFrames { get; set; }
        private ResourceManager manager = Properties.Resources.ResourceManager;
        public DataGridCellInfo SelectedCell { get; set; }

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
                if (item.Supplier_Id == 0)
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
            Supplier newSupplier = (Supplier)this.SelectedCell.Item;
            Supplier oldSupplier = this.Uow.SupplierRepository.Get(filter: s => s.Id == newSupplier.Id, includeProperties: "Town,Country").FirstOrDefault();
            bool cancelled = false;
            bool townChanged = false;
            bool countryChanged = false;
            bool othersChanged = false;
            this.ChangedProperties(oldSupplier, newSupplier, ref townChanged, ref countryChanged, ref othersChanged);
            if (townChanged && !cancelled)
            {
                ChangeTown(ref newSupplier, ref cancelled);
            }
            if (countryChanged && !cancelled)
            {
                ChangeCountry(ref newSupplier, ref cancelled);
            }
            if (othersChanged && !cancelled)
            {
                SaveChanges(ref newSupplier, ref cancelled);
            }
            if (!cancelled)
            {
                if (!countryChanged)
                    newSupplier.Country = null;
                if (!townChanged)
                    newSupplier.Town = null;
                this.Uow.SupplierRepository.Update(newSupplier);
                this.Uow.Save();
            }
            this.FillList();



        }
        private void ChangeTown(ref Supplier newSupplier, ref bool cancelled)
        {
            if (CheckTown(newSupplier.Town))
            {
                var messageBoxResult = MessageBox.Show("Möchten Sie die Änderungen für alle Orte speichern?", "Lieferant Ändern", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    this.Uow.TownRepository.Update(newSupplier.Town);
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    newSupplier.Town_Id = 0;
                    var copiedSupplier = newSupplier;
                    Town existingTown = this.Uow.TownRepository.Get(t => t.TownName == copiedSupplier.Town.TownName && t.ZipCode == copiedSupplier.Town.ZipCode).FirstOrDefault();
                    if (existingTown != null)
                    {
                        newSupplier.Town = existingTown;
                        newSupplier.Town_Id = existingTown.Id;
                    }
                    else
                    {
                        Town newTown = new Town()
                        {
                            TownName = newSupplier.Town.TownName,
                            ZipCode = newSupplier.Town.ZipCode
                        };
                        this.Uow.TownRepository.Insert(newTown);
                        newSupplier.Town = newTown;
                    }

                }
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    cancelled = true;
                }
            }
        }
        private void ChangeCountry(ref Supplier newSupplier, ref bool cancelled)
        {
            var messageBoxResult = MessageBox.Show("Möchten Sie die Änderungen für alle Länder speichern?", "Lieferant Ändern", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Uow.CountryRepository.Update(newSupplier.Country);
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                newSupplier.Country_Id = 0;
                var copiedSupplier = newSupplier;
                Country existingCountry = this.Uow.CountryRepository.Get(c => c.CountryName == copiedSupplier.Country.CountryName).FirstOrDefault();
                if (existingCountry != null)
                {
                    newSupplier.Country = existingCountry;
                    newSupplier.Country_Id = existingCountry.Id;
                }
                else
                {
                    Country newCountry = new Country()
                    {
                        CountryName = newSupplier.Country.CountryName
                    };
                    this.Uow.CountryRepository.Insert(newCountry);
                    newSupplier.Country = newCountry;
                }

            }
            if (messageBoxResult == MessageBoxResult.Cancel)
            {
                cancelled = true;
            }

        }
        private void SaveChanges(ref Supplier newSupplier, ref bool cancelled)
        {
            var messageBoxResult = MessageBox.Show(String.Format("Möchten Sie die Änderungen beim Lieferant mit der Id '{0}' speichern?", newSupplier.Id), "Lieferant Ändern", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (messageBoxResult == MessageBoxResult.No)
            {
                cancelled = true;
            }


        }
        private bool CheckTown(Town town)
        {
            bool check = true;
            if (String.IsNullOrEmpty(town.ZipCode) && !String.IsNullOrEmpty(town.TownName))
            {
                MessageBox.Show("Es muss eine Postleitzahl angegeben werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                check = false;
            }
            if (String.IsNullOrEmpty(town.TownName) && !String.IsNullOrEmpty(town.ZipCode))
            {
                MessageBox.Show("Es muss ein Ortsname angegeben werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                check = false;
            }
            return check;
        }
        private void ChangedProperties(Supplier oldSupplier, Supplier newSupplier, ref bool townChanged, ref bool countryChanged, ref bool othersChanged)
        {
            List<String> props = new List<string>(typeof(Supplier).GetProperties().Select(p => p.Name).ToList());
            props.Remove("Timestamp");
            props.Remove("Id");
            props.Remove("Town_Id");
            props.Remove("Town");
            props.Remove("Country");
            props.Remove("Country_Id");
            props.Add("TownName");
            props.Add("CountryName");
            props.Add("ZipCode");

            foreach (var item in props)
            {
                if (typeof(Town).GetProperty(item) != null)
                {
                    if (oldSupplier.Town?.GetType().GetProperty(item).GetValue(oldSupplier.Town, null)?.ToString() != newSupplier.Town?.GetType().GetProperty(item).GetValue(newSupplier.Town, null)?.ToString())
                    {
                        townChanged = true;
                    }
                }
                else if (typeof(Country).GetProperty(item) != null)
                {
                    if (oldSupplier.Country?.GetType().GetProperty(item).GetValue(oldSupplier.Country, null)?.ToString() != newSupplier.Country?.GetType().GetProperty(item).GetValue(newSupplier.Country, null)?.ToString())
                    {
                        countryChanged = true;
                    }
                }
                else
                {

                    if (oldSupplier.GetType().GetProperty(item).GetValue(oldSupplier, null)?.ToString() != (newSupplier.GetType().GetProperty(item).GetValue(newSupplier, null)?.ToString()))
                    {
                        othersChanged = true;
                    }
                }
            }
        }
    }
}
