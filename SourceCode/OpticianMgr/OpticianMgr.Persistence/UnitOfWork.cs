using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private bool _disposed;

        private GenericRepository<CustomMessage> _messageRepository;

        public IGenericRepository<CustomMessage> MessageRepository
        {
            get
            {
                if (_messageRepository == null)
                    _messageRepository = new GenericRepository<CustomMessage>(_context);
                return _messageRepository;
            }
        }

        private GenericRepository<Supplier> _supplierRepository;

        public IGenericRepository<Supplier> SupplierRepository
        {
            get
            {
                if (_supplierRepository == null)
                    _supplierRepository = new GenericRepository<Supplier>(_context);
                return _supplierRepository;
            }
        }
        private GenericRepository<Town> _townRepository;

        public IGenericRepository<Town> TownRepository
        {
            get
            {
                if (_townRepository == null)
                    _townRepository = new GenericRepository<Town>(_context);
                return _townRepository;
            }
        }
        private GenericRepository<Country> _countryRepository;

        public IGenericRepository<Country> CountryRepository
        {
            get
            {
                if (_countryRepository == null)
                    _countryRepository = new GenericRepository<Country>(_context);
                return _countryRepository;
            }
        }
        private GenericRepository<Doctor> _doctorRepository;

        public IGenericRepository<Doctor> DoctorRepository
        {
            get
            {
                if (_doctorRepository == null)
                    _doctorRepository = new GenericRepository<Doctor>(_context);
                return _doctorRepository;
            }
        }
        private GenericRepository<Order> _orderRepository;

        public IGenericRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new GenericRepository<Order>(_context);
                return _orderRepository;
            }
        }
        private GenericRepository<Glasstype> _glassTypeRepository;

        public IGenericRepository<Glasstype> GlassTypeRepository
        {
            get
            {
                if (_glassTypeRepository == null)
                    _glassTypeRepository = new GenericRepository<Glasstype>(_context);
                return _glassTypeRepository;
            }
        }
        private GenericRepository<ContactLensType> _contactLensTypeRepository;

        public IGenericRepository<ContactLensType> ContactLensTypeRepository
        {
            get
            {
                if (_contactLensTypeRepository == null)
                    _contactLensTypeRepository = new GenericRepository<ContactLensType>(_context);
                return _contactLensTypeRepository;
            }
        }
        private GenericRepository<Customer> _customerRepository;

        public IGenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                    _customerRepository = new GenericRepository<Customer>(_context);
                return _customerRepository;
            }
        }
        private GenericRepository<EyeGlassFrame> _eyeGlassFrameRepository;

        public IGenericRepository<EyeGlassFrame> EyeGlassFrameRepository
        {
            get
            {
                if (_eyeGlassFrameRepository == null)
                    _eyeGlassFrameRepository = new GenericRepository<EyeGlassFrame>(_context);
                return _eyeGlassFrameRepository;
            }
        }
        //TODO due to prefered constructor in viewmodellocator (only one constructor can exist)
        private UnitOfWork(string connectionString)
        {
            _context = new ApplicationDbContext(connectionString);
        }
        public UnitOfWork() : this("name=DefaultConnection")
        {

        }

        /// <summary>
        ///     Repository-übergreifendes Speichern der Änderungen
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void DeleteDatabase()
        {
            _context.Database.Delete();
        }

    }
}
