using GalaSoft.MvvmLight;
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
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> RefreshSuppliers;

        public Supplier Supplier { get; set; }
        public Town Town { get; set; }
        public String NameWarning { get; set; }
        public String TownWarning { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand FindTown { get; set; }
        public ICommand FindZipCode { get; set; }
        public AddSupplierViewModel()
        {
            this.Supplier = new Supplier();
            this.Town = new Town();
            Cancel = new RelayCommand(CancelAddSupplier);
            Submit = new RelayCommand(AddSupplier);
            FindTown = new RelayCommand(FindO);
            FindZipCode = new RelayCommand(FindP);
        }
        private void FindO()
        {
            using (UnitOfWork localUow = new UnitOfWork()) {
                if (!String.IsNullOrEmpty(this.Town.ZipCode))
                {
                    this.Town.TownName = localUow.TownRepository.Get(filter: t => t.ZipCode == this.Town.ZipCode, orderBy: ord => ord.OrderByDescending(or => or.Id)).FirstOrDefault()?.TownName;
                    this.RaisePropertyChanged(() => this.Town);
                }
            }
        }
        private void FindP()
        {
            using(UnitOfWork localUow = new UnitOfWork())
            if (!String.IsNullOrEmpty(this.Town.TownName))
            {
                this.Town.ZipCode = localUow.TownRepository.Get(filter: t => t.TownName == this.Town.TownName, orderBy: ord => ord.OrderByDescending(or => or.Id)).FirstOrDefault()?.ZipCode;
                this.RaisePropertyChanged(() => this.Town);
            }
        }
        public void CancelAddSupplier()
        {
            this.CloseRequested?.Invoke(this, null);
        }
        public void AddSupplier()
        {
            using (UnitOfWork localUow = new UnitOfWork())
            {
                this.NameWarning = "";
                this.TownWarning = "";
                if (this.CheckErrors())
                {
                        Town existingTown = localUow.TownRepository.Get(t => t.TownName == this.Town.TownName && t.ZipCode == this.Town.ZipCode).FirstOrDefault();
                        if (existingTown != null)
                        {
                            this.Supplier.Town = existingTown;
                        }
                        else
                            this.Supplier.Town = this.Town;
                    localUow.SupplierRepository.Insert(this.Supplier);
                    localUow.Save();
                    this.CloseRequested?.Invoke(this, null);
                    this.RefreshSuppliers?.Invoke(this, null);
                }
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
    }
}
