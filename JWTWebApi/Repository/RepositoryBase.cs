using JWTWebApi.Contracts;
using JWTWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JWTWebApi.Contracts
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
       protected RepositoryContext RepositoryContext { get; set; }
 
       public RepositoryBase(RepositoryContext repositoryContext)
       {
           this.RepositoryContext = repositoryContext;
       }
 
       public async Task<IEnumerable<T>> FindAllAsync()
       {
           return await this.RepositoryContext.Set<T>().AsNoTracking().ToListAsync();
       }
 
       public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
       {
           return await this.RepositoryContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();
       }

       public async Task<T> FindByIdAsync(Expression<Func<T, bool>> expression)
       {
           return await this.RepositoryContext.Set<T>().Where(expression).AsNoTracking().FirstOrDefaultAsync();
       }
 
       public void Create(T entity)
       {
           this.RepositoryContext.Set<T>().Add(entity);
       }
 
       public void Update(T entity)
       {
           this.RepositoryContext.Set<T>().Update(entity);
       }
 
       public void Delete(T entity)
       {
           this.RepositoryContext.Set<T>().Remove(entity);
       } 

       public async Task<int> SaveAsync()
       {
           return await this.RepositoryContext.SaveChangesAsync();
       }
    }
}