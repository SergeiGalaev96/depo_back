using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface IRepository<T> where T : Entity
    {
        
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T Get(int? id);
        void Insert(T entity);
        Task<T> InsertAsync(T entity, int user_id);
        Task<T> InsertAsyncWithoutHistory(T entity);
        void Update(T entity, int user_id);
        void UpdateWithoutHistory(T entity);
        void Delete(T entity, int user_id);
        void DeleteWithoutHistory(T entity);
        void Remove(T entity);
        void SaveChanges();
        void Refresh(T entity);
        //Task<int> ExecuteStoredProcedureWithoutHistory(string sqlQuery, params object[] parameters);
        //Task<int> ExecuteStoredProcedureWithoutHistory(string sqlQuery);
    }
}
