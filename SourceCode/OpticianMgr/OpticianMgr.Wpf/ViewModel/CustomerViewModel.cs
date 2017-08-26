using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf.ViewModel
{
    public class CustomerViewModel
    {
        public IUnitOfWork Uow { get; set; }
        public ObservableCollection<Customer> CustomerList { get; set; }
        public CustomerViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
                this.CustomerList = new ObservableCollection<Customer>(this.Uow.CustomerRepository.Get(includeProperties: "Town").ToList());
        }
    }
}
