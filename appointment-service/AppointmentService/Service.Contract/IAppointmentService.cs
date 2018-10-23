using Service.Contract.RequestModel;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IAppointmentService
    {
        Task CreateAppointment(CreateAppointmentRequest request);
    }
}
