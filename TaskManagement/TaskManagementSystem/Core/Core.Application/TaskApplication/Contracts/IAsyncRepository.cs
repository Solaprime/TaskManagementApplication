using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskDomain.Entities;

namespace TaskApplication.Contracts
{
     public  interface IAsyncRepository<T> where T : BaseEntity
     {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SoftDeleteAsync(Guid Id);
        Task<bool> ExistAsync(Guid Id);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);  
     }


}
