using JWTWebApi.Entities;
using JWTWebApi.Contracts;

namespace JWTWebApi.Repository
{
    public class UserRepository: RepositoryBase<UserModel>
    {
        public UserRepository(RepositoryContext repositoryContext): base(repositoryContext){
                
        }
    }
}