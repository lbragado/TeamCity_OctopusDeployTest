using Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.EFCore.DataContext;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Repository.EFCore.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly RepositoryContext Context;

        public UnitOfWork(RepositoryContext context) => Context = context;
        
        public int SaveChanges()
        {
            int result = 0;
            try
            {
                result = Context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }

        public Task<int> SaveChangesAsync()
        {
            Task<int> result = Task.FromResult(0);
            try
            {
                result = Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }
    }
}
