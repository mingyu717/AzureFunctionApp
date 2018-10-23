using Service.Contract.DBModel;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IAppointmentDAL
    {
        Task<int> SaveAppointment(Appointment appointment);
        Task<int> SaveAppointmentJob(AppointmentJob appointmentJob);
    }
}
