using BlazorChat.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlazorChat
{
    public sealed class Repository : IRepository
    {
        readonly ApplicationDbContext dbContext;


        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<T> GetAll<T>() where T : class
        {
            return dbContext.Set<T>();
        }


        public T GetById<T>(int id) where T : class
        {

            return dbContext.Set<T>().Find(id);
        }


        public IQueryable<T> FindBy<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return dbContext.Set<T>().Where(predicate).AsNoTracking();
        }


        public bool IsExist<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return dbContext.Set<T>().Any(predicate);
        }


        public int Save()
        {
            return dbContext.SaveChanges();
        }

        #region Async Versions

        public async Task<int> SaveAsync()
        {
            return await dbContext.SaveChangesAsync();
        }


        public async Task<int> AddAsync<T>(T model) where T : class
        {
            dbContext.Set<T>().Add(model);
            return await SaveAsync();
        }


        public async Task<int> UpdateAsync<T>(T model) where T : class
        {
            dbContext.Entry(model).State = EntityState.Modified;
            return await SaveAsync();
        }


        public async Task<int> DeleteAsync<T>(T model) where T : class
        {
            dbContext.Entry(model).State = EntityState.Deleted;
            return await SaveAsync();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return dbContext.Set<T>().FirstOrDefault(predicate);
        }

        #endregion Async Versions
    }
}
