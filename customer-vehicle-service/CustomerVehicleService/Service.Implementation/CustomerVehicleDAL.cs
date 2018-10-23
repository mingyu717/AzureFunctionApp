using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Service.Contract;

namespace Service.Implementation
{
    public class CustomerVehicleDAL : ICustomerVehicleDAL
    {
        private readonly string _connectionString;

        public CustomerVehicleDAL(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> SaveCustomer(Customer customerDao)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                var existingCustomer = GetCustomer(customerDao.CustomerNo, customerDao.RooftopId, customerDao.CommunityId);
                if (existingCustomer == null)
                {
                    existingCustomer = dbContext.Customers.Add(customerDao);
                }
                else
                {
                    customerDao.Id = existingCustomer.Id;
                    existingCustomer.CustomerEmail = customerDao.CustomerEmail;
                    existingCustomer.FirstName = customerDao.FirstName;
                    existingCustomer.Surname = customerDao.Surname;
                    existingCustomer.PhoneNumber = customerDao.PhoneNumber;
                    dbContext.Customers.AddOrUpdate(existingCustomer);
                }

                await dbContext.SaveChangesAsync();

                return existingCustomer.Id;
            }
        }

        public async Task<int> SaveCustomerVehicle(CustomerVehicle customerVehicleDao)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                var existingCustomerVehicle = GetCustomerVehicle(customerVehicleDao.CustomerId, customerVehicleDao.VehicleNo);

                if (existingCustomerVehicle == null)
                {
                    existingCustomerVehicle = dbContext.CustomerVehicles.Add(customerVehicleDao);
                }
                else
                {
                    existingCustomerVehicle.RegistrationNo = customerVehicleDao.RegistrationNo;
                    existingCustomerVehicle.VinNumber = customerVehicleDao.VinNumber;
                    existingCustomerVehicle.MakeCode = customerVehicleDao.MakeCode;
                    existingCustomerVehicle.ModelCode = customerVehicleDao.ModelCode;
                    existingCustomerVehicle.ModelYear = customerVehicleDao.ModelYear;
                    existingCustomerVehicle.ModelDescription = customerVehicleDao.ModelDescription;
                    existingCustomerVehicle.LastServiceDate = customerVehicleDao.LastServiceDate;
                    existingCustomerVehicle.NextServiceDate = customerVehicleDao.NextServiceDate;
                    existingCustomerVehicle.LastKnownMileage = customerVehicleDao.LastKnownMileage;
                    existingCustomerVehicle.NextServiceMileage = customerVehicleDao.NextServiceMileage;
                    existingCustomerVehicle.VariantCode = customerVehicleDao.VariantCode;
                    dbContext.CustomerVehicles.AddOrUpdate(existingCustomerVehicle);
                }

                await dbContext.SaveChangesAsync();
                return existingCustomerVehicle.Id;
            }
        }

        public Customer GetCustomer(int customerNo, string roofTopId, string communityId)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                return dbContext.Customers.FirstOrDefault(cv =>
                    cv.CustomerNo == customerNo && cv.RooftopId == roofTopId && cv.CommunityId == communityId);
            }
        }

        public CustomerVehicle GetCustomerVehicle(int customerId, int vehicleNo)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                return dbContext.CustomerVehicles.FirstOrDefault(cv =>
                    cv.CustomerId == customerId && cv.VehicleNo == vehicleNo);
            }
        }

        public async Task DeleteCustomerVehicle(int customerId, int vehicleNo)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                var customerVehicles = dbContext.CustomerVehicles.Where(cv =>
                    cv.CustomerId == customerId && cv.VehicleNo == vehicleNo);

                if (customerVehicles.Any())
                {
                    dbContext.CustomerVehicles.RemoveRange(customerVehicles);
                    await dbContext.SaveChangesAsync();
                }
            }

        }

        public async Task LogInvitationDetail(Invitation invitationDao)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                dbContext.Invitations.Add(invitationDao);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task SaveServiceBooking(ServiceBookings serviceBooking)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                dbContext.ServiceBookings.Add(serviceBooking);
                await dbContext.SaveChangesAsync();
            }
        }

        public ServiceBookings GetServiceBooking(int customerId, int vehicleNo, int dealerId)
        {
            using (var dbContext = new CustomerVehicleContext(_connectionString))
            {
                return dbContext.ServiceBookings.Where(x => x.CustomerNo == customerId &&
                                                     x.VehicleNo == vehicleNo &&
                                                     x.DealerId == dealerId)
                    .OrderByDescending(x=> x.CreateTime).FirstOrDefault();
            }
        }
    }
}