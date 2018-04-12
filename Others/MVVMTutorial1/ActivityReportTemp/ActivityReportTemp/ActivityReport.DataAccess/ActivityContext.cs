using ActivityReport.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityReport.DataAccess
{
    public class ActivityContext: DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public ActivityContext(): base(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = ActivityReport; Integrated Security = True")
        {
            Database.SetInitializer(new ActivityContextDbInitializer());
        }

    }

    
}
