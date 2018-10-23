using Service.Contract.Models;
using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICDKAppointmentSlotsService
    {
        Task<GetAppointmentSlotsResponse> GetAppointmentSlots(GetAppointmentSlotsRequest request);
    }
}
