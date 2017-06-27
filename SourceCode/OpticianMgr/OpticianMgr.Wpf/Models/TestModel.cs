using GalaSoft.MvvmLight;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf.Models
{
    public class TestModel : ObservableObject
    {
        private String testName;

        public String TestName
        {
            get { return testName; }
            set { Set<string>(() => this.testName, ref testName, value); }
        }
        public static ObservableCollection<TestEntity> GetTestEntities(IUnitOfWork uow)
        {
            return new ObservableCollection<TestEntity>(uow.TestRepository.Get());
        }
    }
}
