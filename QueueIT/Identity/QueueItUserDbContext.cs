using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QueueIT.Identity
{
    public class QueueItUserDbContext : IdentityDbContext<QueueItUser>
    {
        public QueueItUserDbContext(DbContextOptions<QueueItUserDbContext> options) :base (options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QueueItUser>(user => user.HasIndex(x => x.FirstName).IsUnique(false));
            builder.Entity<QueueItUser>(user => user.HasIndex(x => x.LastName).IsUnique(false));
        }
    }
}