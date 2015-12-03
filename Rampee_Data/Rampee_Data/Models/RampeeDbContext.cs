using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rampee_Data.Models
{
    public class RampeeDbContext : DbContext
    {
        public RampeeDbContext() : base()
        {
        }

        public DbSet<ConnectionRecord> ConnectionRecords { get; set; }
        public DbSet<ConsumerRecord> ConsumerRecords { get; set; }
        public DbSet<MessageRecord> MessageRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
