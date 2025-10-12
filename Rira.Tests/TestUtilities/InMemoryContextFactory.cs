using Microsoft.EntityFrameworkCore;
using Rira.Persistence.Data;

namespace Rira.Tests.TestUtilities
{
    public static class InMemoryContextFactory
    {
        public static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
