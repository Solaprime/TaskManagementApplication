using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskDomain.Entities;

namespace TaskPersistence.Repositories
{
   public class BaseRepository<T>: IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly PersistentDBContext _dbContext;
     
        public BaseRepository(PersistentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async  Task<T> AddAsync(T entity)
        {
            try
            {
                await _dbContext.Set<T>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public async  Task DeleteAsync(T entity)
        {
            try
            {
                _dbContext.Set<T>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

          
        }
        public async Task<bool> ExistAsync(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    
                    throw new ArgumentNullException(nameof(Id));
                }     
                return await  _dbContext.Set<T>().AnyAsync(a => a.Id == Id && a.IsDeleted == false);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual  async Task<T> GetByIdAsync(Guid id)
        {
            try
            {
                return await _dbContext.Set<T>().Where(s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        public virtual  async Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            try
            {
                return await _dbContext.Set<T>().Skip((page - 1) * size).Take(size).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public  async Task<IReadOnlyList<T>> ListAllAsync()
        {
            try
            {
                return await _dbContext.Set<T>().Where(s => s.IsDeleted == false).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task SoftDeleteAsync(Guid Id)
        {
            try
            {
                var result = await GetByIdAsync(Id);
                result.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
           

        }

        public async  Task UpdateAsync(T entity)
        {
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
                entity.LastModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
