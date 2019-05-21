using System;
using JWTWebApi.Contracts;
using JWTWebApi.Entities;
using JWTWebApi.Repository;

namespace TestCoreUow.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly RepositoryContext _context;

        private IRepositoryBase<UserModel> _userRepository;

        public UnitOfWork(RepositoryContext context)
        {
            _context = context;
        }

        public IRepositoryBase<UserModel> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new RepositoryBase<UserModel>(_context));}
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}