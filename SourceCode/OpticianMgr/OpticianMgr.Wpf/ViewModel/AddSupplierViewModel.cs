﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class AddSupplierViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow  { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> RefreshSuppliers;

        public Supplier Supplier { get; set; }
        public Town Town { get; set; }
        public Country Country { get; set; }
        public String NameWarning { get; set; }
        public String TownWarning { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand FindTown { get; set; }
        public ICommand FindCountry { get; set; }
        public ICommand FindZipCode { get; set; }
        public AddSupplierViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.Supplier = new Supplier();
            this.Town = new Town();
            this.Country = new Country();
            Cancel = new RelayCommand(CancelAddSupplier);
            Submit = new RelayCommand(AddSupplier);
            FindTown = new RelayCommand(FindT);
            FindCountry = new RelayCommand(FindC);
            FindZipCode = new RelayCommand(FindZ);
        }
        private void FindT()
        {
            if (!String.IsNullOrEmpty(this.Town.ZipCode))
            {
                this.Town.TownName = this.Uow.TownRepository.Get(filter: t => t.ZipCode == this.Town.ZipCode, orderBy: ord => ord.OrderBy(or => or.Id)).FirstOrDefault()?.TownName;
                this.RaisePropertyChanged(() => this.Town);
            }

        }
        private void FindC()
        {
            if (!String.IsNullOrEmpty(this.Country.CountryName))
            {
                var newCountryName = this.Uow.CountryRepository.Get(filter: c => c.CountryName.ToUpper().IndexOf(this.Country.CountryName.ToUpper()) == 0, orderBy: ord => ord.OrderBy(or => or.Id)).FirstOrDefault()?.CountryName;
                if(newCountryName != null)
                {
                    this.Country.CountryName = newCountryName;
                    this.RaisePropertyChanged(() => this.Country);
                }
                
            }

        }
        private void FindZ()
        {
            if (!String.IsNullOrEmpty(this.Town.TownName))
            {
                this.Town.ZipCode = this.Uow.TownRepository.Get(filter: t => t.TownName == this.Town.TownName, orderBy: ord => ord.OrderBy(or => or.Id)).FirstOrDefault()?.ZipCode;
                this.RaisePropertyChanged(() => this.Town);
            }
        }
        public void CancelAddSupplier()
        {
            this.CloseRequested?.Invoke(this, null);
            this.ResetFields();
        }
        public void AddSupplier()
        {
            this.NameWarning = "";
            this.TownWarning = "";
            if (this.CheckErrors())
            {
                Town existingTown = this.Uow.TownRepository.Get(t => t.TownName == this.Town.TownName && t.ZipCode == this.Town.ZipCode).FirstOrDefault();
                if (existingTown != null)
                {
                    this.Supplier.Town = existingTown;
                }
                else
                    this.Supplier.Town = this.Town;

                Country existingCountry = this.Uow.CountryRepository.Get(c => c.CountryName == this.Country.CountryName).FirstOrDefault();
                if (existingCountry != null)
                    this.Supplier.Country = existingCountry;
                else
                    this.Supplier.Country = this.Country;

                this.Uow.SupplierRepository.Insert(this.Supplier);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.RefreshSuppliers?.Invoke(this, null);
                this.ResetFields();

            }
            this.RaisePropertyChanged(() => this.NameWarning);
            this.RaisePropertyChanged(() => this.TownWarning);
        }
        private bool CheckErrors()
        {
            bool check = true;
            if (String.IsNullOrEmpty(this.Supplier.Name))
            {
                this.NameWarning = "Es muss ein Name angegeben werden!";
                check = false;
            }
            if (String.IsNullOrEmpty(this.Town.ZipCode) && !String.IsNullOrEmpty(this.Town.TownName))
            {
                this.TownWarning = "Es muss eine Postleitzahl angegeben werden!";
                check = false;
            }
            if (String.IsNullOrEmpty(this.Town.TownName) && !String.IsNullOrEmpty(this.Town.ZipCode))
            {
                this.TownWarning = "Es muss ein Ort angegeben werden!";
                check = false;
            }
            return check;
        }
        private void ResetFields()
        {
            this.Town = new Town();
            this.Country = new Country();
            this.RaisePropertyChanged(() => this.Town);
            this.RaisePropertyChanged(() => this.Country);
        }
    }
}
