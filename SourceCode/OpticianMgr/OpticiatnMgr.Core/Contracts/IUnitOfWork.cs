using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Supplier> SupplierRepository { get; }
        IGenericRepository<Town> TownRepository { get; }
        IGenericRepository<Doctor> DoctorRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<Glasstype> GlassTypeRepository { get; }
        IGenericRepository<ContactLensType> ContactLensTypeRepository { get; }
        IGenericRepository<Customer> CustomerRepository { get; }
        IGenericRepository<EyeGlassFrame> EyeGlassFrameRepository { get; }

        void Save();

        void DeleteDatabase();
    }
}
