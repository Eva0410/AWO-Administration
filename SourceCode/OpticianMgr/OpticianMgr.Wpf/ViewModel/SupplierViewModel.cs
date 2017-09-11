﻿using GalaSoft.MvvmLight;
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
    public class SupplierViewModel : ViewModelBase
    {
        public IUnitOfWork Uow { get; set; }
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
        //The properties of the supplier class are safed in English but need to be shown in German
        public ObservableCollection<String> PropertiesList
        {
            get
            {
                ObservableCollection<string> props = new ObservableCollection<string>(typeof(Supplier).GetProperties().Select(p => p.Name).ToList());
                ObservableCollection<string> newList = new ObservableCollection<string>();
                props.Remove("Timestamp"); //Shouldnt be able to filter by timestamp
                props[props.IndexOf("Town_Id")] = "ZipCode"; //shouldnt be able to filter by town_id but user should be able to filter by zipcode
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
        RelayCommand<DataGridCellEditEndingEventArgs> CellEditEndingCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SupplierViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.SupplierList = GetAllSuppliers();
            FilterSuppliers = new RelayCommand(FillSupplierList);
            DeleteFilter = new RelayCommand(DeleteF);
            AddSupplier = new RelayCommand(AddS);
            EditSupplier = new RelayCommand(EditS);
            this.SupplierList.CollectionChanged += this.OnCollectionChanged;
            this.FilterProperty = "Name";
            CellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(args => this.RaisePropertyChanged(() => this.SupplierList));
        }
        //TODO Immer die selbe collection verwenden und nicht jedes Mal neu erstellen (clear collection)
        /// <summary>
        /// Returns a list of the suppliers in the database
        /// All properties must be copied, otherwise the list would reference the unit of work data
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Supplier> GetAllSuppliers()
        {
            var unitOfWorkSuppliers = this.Uow.SupplierRepository.Get().ToList();
            ObservableCollection<Supplier> copiedSuppliers = new ObservableCollection<Supplier>(); 
            copiedSuppliers.CollectionChanged += this.OnCollectionChanged;
            foreach (var item in unitOfWorkSuppliers)
            {
                Supplier sup = new Supplier();
                GenericRepository<Supplier>.CopyProperties(sup, item);
                Town town = new Town(); //Referenced town must be copied as well
                GenericRepository<Town>.CopyProperties(town, this.Uow.TownRepository.GetById(item.Town_Id));
                sup.Town = town;
                copiedSuppliers.Add(sup);
            }
            return copiedSuppliers;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Delete a supplier
            if (e.OldItems != null)
            {
                //OldItems will always only contain one item
                foreach (Supplier item in e.OldItems)
                {
                    var fullItem = this.Uow.SupplierRepository.Get(filter: s => s.Id == item.Id, includeProperties: "EyeGlassFrames").FirstOrDefault();
                    if (fullItem != null)
                    {
                        var messageBoxResult = MessageBox.Show("Wollen Sie den Lieferanten '" + fullItem.Name + "' wirklich löschen?", "Lieferant Löschen", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            this.Uow.SupplierRepository.Delete(fullItem);
                        }

                    }
                }
                this.Uow.Save();
                this.FillSupplierList();
                this.RaisePropertyChanged(() => this.SupplierList);
            }
        }
        /// <summary>
        /// Refreshes and filter the supplier list
        /// </summary>
        public void FillSupplierList()
        {
            IEnumerable<DictionaryEntry> dictionary = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();
            this.FilterProperty = this.FilterProperty == "Ort" ? "Ortsname" : this.FilterProperty; //Program filters by the name of the town and not by by town-object
            string translatedProperty = dictionary.FirstOrDefault(e => e.Value.ToString() == this.FilterProperty).Key?.ToString();
            if (!String.IsNullOrEmpty(this.filterText))
            {
                if (typeof(Town).GetProperty(translatedProperty) != null) //Does the user filter by townname or zipcode?
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
                this.SupplierList = GetAllSuppliers();
            
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        //Delete filter
        public void DeleteF()
        {
            this.SupplierList = GetAllSuppliers();
            this.FilterText = "";
            this.RaisePropertyChanged(() => this.SupplierList);
        }
        //add supplier
        public void AddS()
        {
            //MVVM says that the viewmodel shouldnt know about the view -> service, which shows new windows
            AddSupplierWindowService windowService = new AddSupplierWindowService();
            AddSupplierViewModel viewModel = ViewModelLocator.AddSupplierViewModel;
            windowService.ShowWindow(viewModel);
            EventHandler<EventArgs> refreshSupplierHandler = null;
            refreshSupplierHandler = (sender, e) =>
            {
                viewModel.RefreshSuppliers -= refreshSupplierHandler;
                this.FillSupplierList();
                this.RaisePropertyChanged(() => this.SupplierList);
            };
            viewModel.RefreshSuppliers += refreshSupplierHandler;
        }
        //Edit supplier 
        public void EditS()
        {
            Supplier newSupplier = (Supplier)this.SelectedCell.Item;
            Supplier oldSupplier =  this.Uow.SupplierRepository.Get(filter: s => s.Id == newSupplier.Id, includeProperties: "Town").FirstOrDefault();
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
                            this.Uow.TownRepository.Update(newSupplier.Town);
                            this.Uow.Save();
                        }
                        else if (messageBoxResult == MessageBoxResult.No)
                        {
                            newSupplier.Town_Id = 0;
                            Town existingTown = this.Uow.TownRepository.Get(t => t.TownName == newSupplier.Town.TownName && t.ZipCode == newSupplier.Town.ZipCode).FirstOrDefault();
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
                            this.Uow.SupplierRepository.Update(newSupplier);
                            this.Uow.Save();

                        }
                        this.FillSupplierList();
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
            var messageBoxResult = MessageBox.Show(String.Format("Möchten Sie die Änderungen beim Lieferant mit der Id '{0}' speichern?", newSupplier.Id), "Lieferant Ändern", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                newSupplier.Town = null;
                this.Uow.SupplierRepository.Update(newSupplier);
                this.Uow.Save();
            }
            this.FillSupplierList();

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
