using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ActivityReport.Entity;

namespace ActivityReport.DataAccess
{
    class ActivityContextDbInitializer: DropCreateDatabaseAlways<ActivityContext>
    {
        protected override void Seed(ActivityContext context)
        {
            base.Seed(context);

            Employee emp = new Employee() { Id = 1, FirstName = "Max", LastName = "Mustermann" };
            context.Employees.Add(emp);
            context.Employees.Add(new Employee() { Id = 2, FirstName = "Sarah", LastName = "Aigner" });
        }
    }
}
