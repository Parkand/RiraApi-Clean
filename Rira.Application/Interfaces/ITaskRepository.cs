using System.Collections.Generic;
using System.Threading.Tasks;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// اینترفیس ریپازیتوری برای ارتباط با دیتابیس مربوط به تسک‌ها
    /// </summary>
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<TaskEntity> CreateAsync(TaskEntity entity);
        Task<bool> UpdateAsync(TaskEntity entity);
        Task<bool> DeleteAsync(int id);
    }
}
