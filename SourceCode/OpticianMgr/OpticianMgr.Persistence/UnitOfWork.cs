using OpticiatnMgr.Core.Contracts;
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
        ///// TODO maybe add further Repositories
        //private GenericRepository<Tutor> _tutorRepository;

        //public IGenericRepository<Tutor> TutorRepository
        //{
        //    get
        //    {
        //        if (_tutorRepository == null)
        //            _tutorRepository = new GenericRepository<Tutor>(_context);
        //        return _tutorRepository;
        //    }
        //}

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
