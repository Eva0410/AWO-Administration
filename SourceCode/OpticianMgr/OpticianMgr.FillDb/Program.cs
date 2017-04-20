using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.FillDb
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Test Db ersetzen
            using (UnitOfWork uow = new UnitOfWork())
            {
                var test = new List<TestEntity>();
                TestEntity t1 = new TestEntity();
                t1.Test = 1;
                TestEntity t2 = new TestEntity();
                t2.Test = 2;
                test.Add(t1);
                test.Add(t2);

                uow.TestRepository.InsertMany(test);
                uow.Save();

                var test2 = uow.TestRepository.Get();

                foreach (var item in test2)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.ReadKey();
            }
        }
    }
}
