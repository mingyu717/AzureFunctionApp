using AutoMapper;
using Service.Contract;
using Service.Contract.DBModel;
using Service.Contract.RequestModel;
using System;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentDAL _appointmentDAL;
        private IMapper _mapper;

        public AppointmentService(IAppointmentDAL appointmentDAL, IMapper mapper)
        {
            _appointmentDAL = appointmentDAL ?? throw new ArgumentNullException(nameof(appointmentDAL));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task CreateAppointment(CreateAppointmentRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var appointment = MapAppointment(request);
            var appointmentJob = MapAppointmentJob(request);
            var appointmentId = await _appointmentDAL.SaveAppointment(appointment);
            appointmentJob.AppointmentId = appointmentId;
            await _appointmentDAL.SaveAppointmentJob(appointmentJob);
        }

        internal Appointment MapAppointment(CreateAppointmentRequest request)
        {
            return _mapper.Map<CreateAppointmentRequest, Appointment>(request);
        }
         
        internal AppointmentJob MapAppointmentJob(CreateAppointmentRequest request)
        {
            return _mapper.Map<CreateAppointmentRequest, AppointmentJob>(request);
        }
    }
}
