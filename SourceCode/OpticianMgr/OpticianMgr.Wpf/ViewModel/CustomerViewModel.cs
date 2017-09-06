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

namespace OpticianMgr.Wpf.ViewModel
{
    public class CustomerViewModel
    {
        private IUnitOfWork Uow { get; set; }

        public ICommand EditSupplier { get; set; }
        public ObservableCollection<Customer> CustomerList { get; set; }
        public CustomerViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            var customers = new List<Customer>(this.Uow.CustomerRepository.Get(includeProperties: "Town").ToList());
            this.CustomerList = new ObservableCollection<Customer>();
            EditSupplier = new RelayCommand(EditS);
            foreach (var item in customers)
            {
                var cust = new Customer();
                GenericRepository<Customer>.CopyProperties(cust, item);
                cust.Town = this.Uow.TownRepository.GetById(item.Town_Id);
                this.CustomerList.Add(cust);
            }
        }
        public void EditS()
        {
            var tmp = this.Uow.CustomerRepository.Get();
        }
    }
}
