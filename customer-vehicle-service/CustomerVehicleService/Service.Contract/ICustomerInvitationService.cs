using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICustomerInvitationService
    {
        Task<CommunicationMethod> Invite(SaveCustomerVehicleRequest customerVehicle, DealerConfigurationResponse dealerConfigResponse, CommunicationMethod method);
        bool ValidateLink(CustomerVehicle customerVehicle);
    }
}