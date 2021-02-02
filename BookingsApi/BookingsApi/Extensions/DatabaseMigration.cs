using BookingsApi.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookingsApi.Extensions
{
    public static class DatabaseMigration
    {
        public static void RunLatestMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var ctx = serviceScope.ServiceProvider.GetService<BookingsDbContext>();
                ctx.Database.Migrate();
            }
        }
    }
}