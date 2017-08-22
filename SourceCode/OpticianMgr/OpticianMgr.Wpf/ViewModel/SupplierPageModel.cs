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
    public class SupplierPageModel : ViewModelBase
    {
        private ObservableCollection<Supplier> supplierList;
        private ResourceManager manager = Properties.Resources.ResourceManager;

        private DataGridCellInfo _selectedCell;

        public DataGridCellInfo SelectedCell
        {
            get { return _selectedCell; }
            set { _selectedCell = value; }
        }

        public ObservableCollection<Supplier> SupplierList
        {
            get
            {
                return supplierList;
            }
            set
            {
                supplierList = value;
            }
        }
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Supplier).GetProperties().Select(p => p.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp");
                props[props.IndexOf("Town_Id")] = "ZipCode";
                foreach (var item in props)
                {
                    var germanItem = manager.GetString(item);
                    if (germanItem != null)
                        newList.Add(germanItem);
                }
                return newList;
            }
        }
        private string filterProperty;

        public string FilterProperty
        {
            get { return filterProperty; }
            set { filterProperty = value; }
        }
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


        public ICommand FilterSuppliers { get; set; }
        public ICommand DeleteFilter { get; set; }
        public ICommand AddSupplier { get; set; }
        public ICommand EditSupplier { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierPageModel()
        {
            UpdateSuppliers();
            FilterSuppliers = new RelayCommand(Filter);
            DeleteFilter = new RelayCommand(DeleteF);
            AddSupplier = new RelayCommand(AddS);
            EditSupplier = new RelayCommand(EditS);
            this.SupplierList.CollectionChanged += this.OnCollectionChanged;
        }
        private void UpdateSuppliers()
        {
            this.supplierList = this.GetAllSuppliers();
        }
        //TODO Immer die selbe collection verwenden und nicht jedes Mal neu erstellen
        private ObservableCollection<Supplier> GetAllSuppliers()
        {
            ObservableCollection<Supplier> collection = new ObservableCollection<Supplier>();
            using (UnitOfWork localUOW = new UnitOfWork())
            {
                collection = new ObservableCollection<Supplier>(localUOW.SupplierRepository.Get(includeProperties: "Town").ToList());
                collection.CollectionChanged += this.OnCollectionChanged;
            }
            return collection;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Delete Items
            if (e.OldItems != null)
            {
                foreach (Supplier item in e.OldItems)
                {
                    Supplier existingItem = null;
                    using (UnitOfWork localUOW = new UnitOfWork())
                    {
                        existingItem = localUOW.SupplierRepository.GetById(item.Id);
                    }
                    if (item.Id != 0 && existingItem != null)
                    {
                        var messageBoxResult = MessageBox.Show("Wollen Sie den Lieferanten '" + item.Name + "' wirklich löschen?", "Lieferant Löschen", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            using (UnitOfWork localUOW = new UnitOfWork())
                            {
                                localUOW.SupplierRepository.Delete(item);
                                localUOW.Save(); //Sollte nach der for-schleife passieren, ist aber wegen localUOW nicht möglich
                            }
                        }

                    }
                }
                UpdateSuppliers();
                this.RaisePropertyChanged(() => this.SupplierList);
            }
        }

        public void Filter()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterProperty = this.FilterProperty == "Ort" ? "Ortsname" : this.FilterProperty;
            string translatedProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            if (this.filterText != "")
            {
                if (typeof(Town).GetProperty(translatedProperty) != null)
                {
                    this.SupplierList = new ObservableCollection<Supplier>(GetAllSuppliers().Where(s => s.Town?.GetType().GetProperty(translatedProperty).GetValue(s.Town, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));

                }
                else
                {
                    this.SupplierList = new ObservableCollection<Supplier>(GetAllSuppliers().Where(s => s.GetType().GetProperty(translatedProperty).GetValue(s, null)?.ToString().ToUpper().IndexOf(this.filterText.ToUpper()) >= 0));
                }
                this.SupplierList.CollectionChanged += OnCollectionChanged;
            }
            else
                UpdateSuppliers();
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        //Delete filter
        public void DeleteF()
        {
            UpdateSuppliers();
            this.FilterText = "";
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        //add supplier
        public void AddS()
        {
            AddSupplierWindowService windowService = new AddSupplierWindowService();
            AddSupplierViewModel viewModel = new AddSupplierViewModel();
            windowService.ShowWindow(viewModel);
            EventHandler<EventArgs> refreshSupplierHandler = null;
            refreshSupplierHandler = (sender, e) =>
            {
                viewModel.RefreshSuppliers -= refreshSupplierHandler;
                this.UpdateSuppliers();
                this.RaisePropertyChanged(() => this.SupplierList);
            };
            viewModel.RefreshSuppliers += refreshSupplierHandler;
        }
        //Edit supplier 
        public void EditS()
        {
            Supplier newSupplier = (Supplier)this.SelectedCell.Item;
            Supplier oldSupplier = null;
            using (UnitOfWork localUow = new UnitOfWork())
            {
                oldSupplier = localUow.SupplierRepository.Get(filter: s => s.Id == newSupplier.Id, includeProperties: "Town").FirstOrDefault();
            }
            List<String> changedProperties = this.ChangedProperties(oldSupplier, newSupplier);
            if (changedProperties.Count != 0)
            {
                if (changedProperties.Contains("ZipCode") || changedProperties.Contains("TownName"))
                {
                    if (CheckTown(newSupplier.Town))
                    {
                        var messageBoxResult = MessageBox.Show("Möchten Sie die Änderungen für alle Orte speichern?", "Lieferant Ändern", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            using (UnitOfWork localUow = new UnitOfWork())
                            {
                                localUow.TownRepository.Update(newSupplier.Town);
                                localUow.Save();
                            }
                        }
                        else if (messageBoxResult == MessageBoxResult.No)
                        {
                            using (UnitOfWork localUow = new UnitOfWork())
                            {
                                newSupplier.Town_Id = 0;
                                Town existingTown = localUow.TownRepository.Get(t => t.TownName == newSupplier.Town.TownName && t.ZipCode == newSupplier.Town.ZipCode).FirstOrDefault();
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
                                    localUow.TownRepository.Insert(newTown);
                                    newSupplier.Town = newTown;
                                }
                                localUow.SupplierRepository.Update(newSupplier);
                                localUow.Save();
                            }
                        }
                        UpdateSuppliers();
                        this.RaisePropertyChanged(() => this.SupplierList);
                    }
                }
                else
                {
                    SaveChanges(newSupplier);
                }
            }


        }
        private void SaveChanges(Supplier newSupplier)
        {
            using (UnitOfWork localUow = new UnitOfWork())
            {
                var messageBoxResult = MessageBox.Show(String.Format("Möchten Sie die Änderungen beim Lieferant mit der Id '{0}' speichern?", newSupplier.Id), "Lieferant Ändern", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    localUow.SupplierRepository.Update(newSupplier);
                    localUow.Save();
                }
            }
            UpdateSuppliers();
            this.RaisePropertyChanged(() => this.SupplierList);
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
        private List<String> ChangedProperties(Supplier oldSupplier, Supplier newSupplier)
        {
            List<String> changed = new List<string>();
            List<String> props = new List<string>(typeof(Supplier).GetProperties().Select(p => p.Name).ToList());
            props.Remove("Timestamp");
            props.Remove("Id");
            props.Remove("Town_Id");
            props.Remove("Town");
            props.Add("TownName");
            props.Add("ZipCode");
            foreach (var item in props)
            {
                if (typeof(Town).GetProperty(item) != null)
                {
                    if (oldSupplier.Town?.GetType().GetProperty(item).GetValue(oldSupplier.Town, null)?.ToString() != newSupplier.Town?.GetType().GetProperty(item).GetValue(newSupplier.Town, null)?.ToString())
                    {
                        changed.Add(item);
                    }
                }
                else
                {

                    if (oldSupplier.GetType().GetProperty(item).GetValue(oldSupplier, null)?.ToString() != (newSupplier.GetType().GetProperty(item).GetValue(newSupplier, null)?.ToString()))
                    {
                        changed.Add(item);
                    }
                }
            }
            return changed;

        }
    }
}
