using Service.Contract.DBModel;
using System.Data.Entity;

namespace Service.Implementation
{
    public class AppointmentContext:DbContext
    {
        protected AppointmentContext()
        {

        }
        public AppointmentContext(string connectionString):base(connectionString)
        {

        }

        public virtual IDbSet<Appointment> Appointments { get; set; }
        public virtual IDbSet<AppointmentJob> AppointmentJobs { get; set; }
    }
}
