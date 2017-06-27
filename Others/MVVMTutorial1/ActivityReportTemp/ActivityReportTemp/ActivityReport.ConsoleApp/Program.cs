using ExamManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityReport.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var uow = new UnitOfWork())
            {
                var res = uow.EmployeeRepository.Get();
                Console.WriteLine("Personen in der Datenbank:");
                Console.WriteLine("--------------------------");
                foreach (var item in res)
                {
                    Console.WriteLine(item.FirstName+' '+item.LastName);
                }
            }
        }
    }
}
