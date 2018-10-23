using Service.Contract;
using System.Data.Entity;

namespace Service.Implementation
{
    public partial class ConfigurationManagerContext : DbContext
    {
        public DbSet<DealerConfiguration> DealConfigurations { get; set; }
    }
}
