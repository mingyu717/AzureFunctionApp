using System.Data.Entity;
using Service.Contract;

namespace Service.Implementation
{
    public class CustomerVehicleContext : DbContext
    {
        public CustomerVehicleContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerVehicle> CustomerVehicles { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<ServiceBookings> ServiceBookings { get; set; }
    }
}