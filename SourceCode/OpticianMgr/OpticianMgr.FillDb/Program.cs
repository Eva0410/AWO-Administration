﻿using OpticianMgr.Persistence;
using OpticiatnMgr.Core;
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
                OpticianController controller = new OpticianController(uow);
                controller.FillDatabaseFromCsv();
                foreach (var item in uow.SupplierRepository.Get())
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine("Alle Daten wurden erfolgreich eingefügt!");
                Console.ReadKey();
            }
        }
    }
}
