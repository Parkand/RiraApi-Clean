using Microsoft.EntityFrameworkCore;
using Rira.Domain.Entities;
using Rira.Application.Interfaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Persistence.Data
{
    /// <summary>
    /// ✅ DbContext اصلی پروژهٔ ریرا، هماهنگ با EF Core 8 و تست‌های Mock.
    /// شامل متد جدید Set<TEntity>() و پوشش کامل Interface IAppDbContext.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }

        // اصلاح برای جلوگیری از CS1061
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
            => base.Set<TEntity>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
