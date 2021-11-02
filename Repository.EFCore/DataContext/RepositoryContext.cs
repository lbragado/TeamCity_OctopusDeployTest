using Entities.POCOs;
using Microsoft.EntityFrameworkCore;

namespace Repository.EFCore.DataContext
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options) { }
        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CurrentBalance> CurrentBalances { get; set; }
        public DbSet<Operation> Operations { get; set; }
    }
}
