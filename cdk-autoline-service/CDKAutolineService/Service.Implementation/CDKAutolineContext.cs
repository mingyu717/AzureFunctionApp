using System.Data.Entity;
using Service.Contract.DbModels;

namespace Service.Implementation
{
    public class CDKAutolineContext : DbContext
    {
        protected CDKAutolineContext()
        {
        }

        public CDKAutolineContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CDKAutolineContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Map entity to table
            modelBuilder.Entity<AppToken>().ToTable("AppTokens");
            modelBuilder.Entity<CdkCustomer>().ToTable("CdkCustomer");
            modelBuilder.Entity<DealerCDKConfiguration>().ToTable("DealerCDKConfigurations");
        }

        public virtual IDbSet<AppToken> AppTokens { get; set; }
        public virtual IDbSet<CdkCustomer> CdkCustomers { get; set; }
        public virtual IDbSet<DealerCDKConfiguration> DealerCDKConfigurations { get; set; }
    }
}