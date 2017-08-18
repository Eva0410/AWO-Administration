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
        private IUnitOfWork uow;

        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> RefreshSuppliers;

        public Lieferant Lieferant { get; set; }
        public Ort Ort { get; set; }
        public String NameWarning { get; set; }
        public String OrtWarning { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand FindOrt { get; set; }
        public ICommand FindPLZ { get; set; }
        public AddSupplierViewModel()
        {
            this.uow = new UnitOfWork();
            this.Lieferant = new Lieferant();
            this.Ort = new Ort();
            Cancel = new RelayCommand(CancelAddSupplier);
            Submit = new RelayCommand(AddSupplier);
            FindOrt = new RelayCommand(FindO);
            FindPLZ = new RelayCommand(FindP);
        }
        private void FindO()
        {
            if(!String.IsNullOrEmpty(this.Ort.PLZ))
            {
                this.Ort.OrtName = this.uow.OrtRepository.Get(filter: o => o.PLZ == this.Ort.PLZ, orderBy: ord => ord.OrderByDescending(or => or.Id)).FirstOrDefault()?.OrtName;
                this.RaisePropertyChanged(() => this.Ort);
            }
        }
        private void FindP()
        {
            if (!String.IsNullOrEmpty(this.Ort.OrtName))
            {
                this.Ort.PLZ = this.uow.OrtRepository.Get(filter: o => o.OrtName == this.Ort.OrtName, orderBy: ord => ord.OrderByDescending(or => or.Id)).FirstOrDefault()?.PLZ;
                this.RaisePropertyChanged(() => this.Ort);
            }
        }
        private void updateTextBox()
        {
            
        }
        public void CancelAddSupplier()
        {
            this.CloseRequested?.Invoke(this, null);
        }
        public void AddSupplier()
        {
            this.NameWarning = "";
            this.OrtWarning = "";
            if(this.checkErrors())
            {
                if (String.IsNullOrEmpty(this.Ort.OrtName) || String.IsNullOrEmpty(this.Ort.PLZ))
                {
                    this.Lieferant.Ort = null;
                }
                else
                {
                    this.Lieferant.Ort = this.Ort;
                }
                this.uow.LieferantenRepository.Insert(this.Lieferant);
                this.uow.Save();
                this.CloseRequested?.Invoke(this,null);
                this.RefreshSuppliers?.Invoke(this, null);
            }
            this.RaisePropertyChanged(() => this.NameWarning);
            this.RaisePropertyChanged(() => this.OrtWarning);
        }
        private  bool checkErrors()
        {
            bool check = true;
            if (String.IsNullOrEmpty(this.Lieferant.Lieferantenname))
            {
                this.NameWarning = "Es muss ein Name angegeben werden!";
                check = false;
            }
            if (String.IsNullOrEmpty(this.Ort.PLZ) && !String.IsNullOrEmpty(this.Ort.OrtName))
            {
                this.OrtWarning = "Es muss eine Postleitzahl angegeben werden!";
                check = false;
            }
            if (String.IsNullOrEmpty(this.Ort.OrtName) && !String.IsNullOrEmpty(this.Ort.PLZ))
            {
                this.OrtWarning = "Es muss ein Ort angegeben werden!";
                check = false;
            }
            return check;
        }
    }
}
