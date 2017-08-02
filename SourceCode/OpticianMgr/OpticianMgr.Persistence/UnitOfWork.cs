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

        //TODO add repositories
        ///// <summary>
        /////     Konkrete Repositories. Keine Ableitung erforderlich
        ///// </summary>
        private GenericRepository<TestEntity> _testRepository;

        public IGenericRepository<TestEntity> TestRepository
        {
            get
            {
                if (_testRepository == null)
                    _testRepository = new GenericRepository<TestEntity>(_context);
                return _testRepository;
            }
        }
        private GenericRepository<Lieferant> _lieferantenRepository;

        public IGenericRepository<Lieferant> LieferantenRepository
        {
            get
            {
                if (_lieferantenRepository == null)
                    _lieferantenRepository = new GenericRepository<Lieferant>(_context);
                return _lieferantenRepository;
            }
        }
        private GenericRepository<Ort> _ortRepository;

        public IGenericRepository<Ort> OrtRepository
        {
            get
            {
                if (_ortRepository == null)
                    _ortRepository = new GenericRepository<Ort>(_context);
                return _ortRepository;
            }
        }

        public UnitOfWork(string connectionString)
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
