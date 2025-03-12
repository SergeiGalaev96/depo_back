using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;


using Newtonsoft.Json;

namespace Depository.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected ApplicationDbContext _context;
        const int CREATE_EVENT_ID = 1;
        const int UPDATE_EVENT_ID = 2;
        const int DELETE_EVENT_ID = 3;
        protected DbSet<T> DbSet { get; set; }

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }


        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }


        public void Update(T entity, int user_id)
        {

            DbSet.Update(entity);
            var rrr = JsonConvert.SerializeObject(entity);
            _context.history.AddAsync(new history { created_at = DateTime.Now, updated_at = DateTime.Now, event_id = UPDATE_EVENT_ID, object_id = entity.id, object_str = JsonConvert.SerializeObject(entity), object_name = entity.GetType().Name, user_id = user_id, deleted = false });
            _context.SaveChanges();
            
        }

      
        public void Delete(T entity, int user_id)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Update(entity);
            _context.history.AddAsync(new history { created_at = DateTime.Now, updated_at = DateTime.Now, event_id = DELETE_EVENT_ID, object_id = entity.id, object_str = JsonConvert.SerializeObject(entity), object_name = entity.GetType().Name, user_id = user_id, deleted = false });
            _context.SaveChanges();
        }

        public virtual T Get(int? Id)
        {
            return DbSet.Where(x => x.id == Id).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task<T> InsertAsync(T entity, int user_id)
        {
            var entry = await DbSet.AddAsync(entity);
            _context.SaveChanges();
            await _context.history.AddAsync(new history { created_at = DateTime.Now, updated_at = DateTime.Now, event_id = CREATE_EVENT_ID, object_id = entry.Entity.id, object_str = JsonConvert.SerializeObject(entity), object_name = entity.GetType().Name, user_id = user_id, deleted = false });
           
            return entry.Entity;
        }

        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Refresh(T entity)
        {
            _context.Entry(entity).Reload();
        }

       



        public async Task<T> InsertAsyncWithoutHistory(T entity)
        {
            var entry = await DbSet.AddAsync(entity);
            _context.SaveChanges();
            return entry.Entity;
        }

        public void UpdateWithoutHistory(T entity)
        {
            DbSet.Update(entity);
            _context.SaveChanges();
        }

        public void DeleteWithoutHistory(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
