using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICustomerVehicleService
    {
        Task SaveCustomerVehicle(SaveCustomerVehicleRequest request, DealerConfigurationResponse dealerConfigResponse);
        Task<GetCustomerVehicleResponse> GetCustomerVehicle(int customerId, int vehicleId, DealerConfigurationResponse dealerConfigResponse);
        Task DismissVehicleOwnership(DismissVehicleOwnershipRequest request, DealerConfigurationResponse dealerConfigResponse);
        Task UpdateCustomerContact(UpdateCustomerContactRequest request, DealerConfigurationResponse dealerConfigurationResponse);
        Task<CreateServiceBookingResponse> CreateServiceBooking(CreateServiceBookingRequest request, DealerConfigurationResponse dealerConfigurationResponse);
        GetCustomerVehicleResponse ExistingServiceBooking(int customerId, int vehicleId, int dealerid);
    }
}