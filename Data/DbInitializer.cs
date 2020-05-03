using Microsoft.EntityFrameworkCore;

namespace ImmageAggregatorAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IAAPIDbContext context)
        {
            context.Database.Migrate();
        }
    }
}