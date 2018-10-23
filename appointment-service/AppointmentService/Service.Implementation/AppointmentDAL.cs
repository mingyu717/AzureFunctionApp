using Service.Contract;
using Service.Contract.DBModel;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class AppointmentDAL : IAppointmentDAL
    {
        private readonly AppointmentContext _context;

        public AppointmentDAL(AppointmentContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment.Id;
        }

        public async Task<int> SaveAppointmentJob(AppointmentJob appointmentJob)
        {
            _context.AppointmentJobs.Add(appointmentJob);
            await _context.SaveChangesAsync();
            return appointmentJob.Id;
        }
    }
}
