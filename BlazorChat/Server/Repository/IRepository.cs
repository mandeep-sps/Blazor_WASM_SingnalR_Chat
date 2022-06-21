using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlazorChat
{
    public interface IRepository
    {
        Task<int> AddAsync<T>(T model) where T : class;
        Task<int> DeleteAsync<T>(T model) where T : class;
        IQueryable<T> FindBy<T>(Expression<Func<T, bool>> predicate) where T : class;
        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> GetAll<T>() where T : class;
        T GetById<T>(int id) where T : class;
        bool IsExist<T>(Expression<Func<T, bool>> predicate) where T : class;
        int Save();
        Task<int> SaveAsync();
        Task<int> UpdateAsync<T>(T model) where T : class;
    }
}