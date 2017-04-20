using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        //TODO add further DBSets
        //public DbSet<Tutor> Tutors { get; set; }


        public ApplicationDbContext() : base("name=DefaultConnection")
        {

        }

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
    }
}
