using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICustomerVehicleDAL
    {
        Task<int> SaveCustomer(Customer customerDao);
        Task<int> SaveCustomerVehicle(CustomerVehicle customerVehicleDao);
        Customer GetCustomer(int customerId, string rooftopId, string communityId);
        CustomerVehicle GetCustomerVehicle(int customerId, int vehicleId);
        Task LogInvitationDetail(Invitation invitationDao);
        Task DeleteCustomerVehicle(int customerId, int vehicleNo);
        Task SaveServiceBooking(ServiceBookings serviceBooking);
        ServiceBookings GetServiceBooking(int customerId, int vehicleNo, int dealerId);
    }
}