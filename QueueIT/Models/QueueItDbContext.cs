using Microsoft.EntityFrameworkCore;
using QueueIT.Notifications;

namespace QueueIT.Models
{
    public class QueueItDbContext : DbContext
    {
        public QueueItDbContext (DbContextOptions options)
            : base(options)
        { }
        
        public DbSet<Queue> Queues { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamQueue> TeamsQueues { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTeam>().HasKey(u => new {u.UserId, u.TeamId});
            modelBuilder.Entity<TeamQueue>().HasKey(tq => new {tq.TeamId, tq.QueueId});
        }
    }
}
