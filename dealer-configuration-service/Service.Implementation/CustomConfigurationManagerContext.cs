using Service.Contract;
using System.Data.Entity;

namespace Service.Implementation
{
    public partial class ConfigurationManagerContext : DbContext
    {
        public ConfigurationManagerContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ConfigurationManagerContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Map entity to table
            modelBuilder.Entity<DealerConfiguration>().ToTable("DealerConfiguration");
        }
    }
}
