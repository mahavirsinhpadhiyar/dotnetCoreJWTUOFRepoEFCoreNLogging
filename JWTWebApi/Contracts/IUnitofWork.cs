using JWTWebApi.Entities;

namespace JWTWebApi.Contracts
{
    public interface IUnitOfWork
    {
        IRepositoryBase<UserModel> UserRepository { get; }
        // IGenericRepository<Reports> ReportRepository { get; }
        // IGenericRepository<Extensions> ExtensionRepository { get; }
        // IGenericRepository<Comments> CommentRepository { get; }
        // IGenericRepository<AuditLog> AuditRepository { get; }
        void Save();
    }
}