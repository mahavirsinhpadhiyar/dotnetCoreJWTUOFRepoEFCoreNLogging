using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace JWTWebApi.Entities
{
    public class RepositoryContext: DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            :base(options)
        { 
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //     if(!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseNpgsql(@"host=server;database=test;user id=postgres;");
        //     }
        // }
        public DbSet<UserModel> Users { get; set; }
    }
}