using OpticiatnMgr.Core.Entities;
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
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Glasstype> GlassTypes { get; set; }
        public DbSet<ContactLensType> ContactLensTypes { get; set; }
        public DbSet<EyeGlassFrame> EyeGlassFrames { get; set; }
        public DbSet<Customer> Customers { get; set; }


        public ApplicationDbContext() : base("name=DefaultConnection")
        {

        }

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
    }
}
