using ActivityReport.DataAccess;
using ActivityReport.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.DataAccess
{
    public class UnitOfWork : IDisposable
    {
        private readonly ActivityContext _context = new ActivityContext();
        private bool _disposed;

        /// <summary>
        ///     Konkrete Repositories. Keine Ableitung erforderlich
        /// </summary>
        private GenericRepository<Employee> _employeeRepository;
        

        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (_employeeRepository == null)
                {
                    _employeeRepository = new GenericRepository<Employee>(_context);
                }
                return _employeeRepository;
            }
        }
       
        /// <summary>
        ///     Repository-übergreifendes Speichern der Änderungen
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        public void DeleteDatabase()
        {
            _context.Database.Delete();
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
    }
}
