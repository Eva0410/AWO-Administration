using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticianMgr.Wpf.WindowServices;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class GlassTypeDetailsViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public event EventHandler<EventArgs> CloseRequested;
        public event EventHandler<EventArgs> Refresh;
        public Glasstype Glasstype { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Submit { get; set; }
        public ICommand Delete { get; set; }
        public GlassTypeDetailsViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            Cancel = new RelayCommand(CancelEditGlasstype);
            Submit = new RelayCommand(SaveGlassType);    
            Delete = new RelayCommand(DeleteG);
        }
        public void InitGlassType(int id)
        {
            var uowGlasstype = this.Uow.GlassTypeRepository.GetById(id);
            Glasstype newGlasstype = new Glasstype();
            GenericRepository<Glasstype>.CopyProperties(newGlasstype, uowGlasstype);
            this.Glasstype = newGlasstype;
            RaisePropertyChanged(() => this.Glasstype);
        }

        public void DeleteG()
        {
            var result = MessageBox.Show(String.Format("Wollen Sie den Glastyp '{0}' wirklich löschen? {1} Achtung! {1} {2} Bestellungen verweisen auf diesen Glastyp. {1}Wenn Sie den Glastyp dennoch löschen bleibt bei diesen Bestellungen der Glastyp leer.", this.Glasstype.GlasstypeDescription, Environment.NewLine, this.Uow.OrderRepository.Count(filter: o => o.GlassType_Id == this.Glasstype.Id)), "Glastyp löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                var g = this.Uow.GlassTypeRepository.GetById(this.Glasstype.Id);
                this.Uow.GlassTypeRepository.Delete(g);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
        }
        
        public void CancelEditGlasstype()
        {
            this.CloseRequested?.Invoke(this, null);
            this.Refresh?.Invoke(this, null);
        }
        public void SaveGlassType()
        {
            try
            {
                this.Uow.GlassTypeRepository.Update(this.Glasstype);
                this.Uow.Save();
                this.CloseRequested?.Invoke(this, null);
                this.Refresh?.Invoke(this, null);
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += validationError.ErrorMessage;
                        message += "\n";
                    }
                }
                MessageBox.Show(message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}

