using System;
using System.Threading.Tasks;
using AutoMapper;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Response;

namespace Service.Implementation
{
    public class CustomerVehicleService : ICustomerVehicleService
    {
        private readonly ICustomerVehicleDAL _customerVehicleDal;
        private readonly IMapper _mapper;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerInvitationService _customerInvitationService;
        private readonly IEmailService _emailService;
        private readonly ICustomerServiceBooking _customerServiceBooking;
        private readonly int _serviceBookingExpiredDays;

        public CustomerVehicleService(ICustomerVehicleDAL customerVehicleDal, IMapper mapper,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerInvitationService customerInvitationService, IEmailService emailService,
            ICustomerServiceBooking customerBookingService, int serviceBookingExpiredDays)
        {
            if (serviceBookingExpiredDays <= 0) throw new ArgumentOutOfRangeException(nameof(serviceBookingExpiredDays));
            _customerVehicleDal = customerVehicleDal ?? throw new ArgumentNullException(nameof(customerVehicleDal));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerRegistrationService = customerRegistrationService ??
                                           throw new ArgumentNullException(nameof(customerRegistrationService));
            _customerInvitationService = customerInvitationService ??
                                         throw new ArgumentNullException(nameof(customerInvitationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _customerServiceBooking = customerBookingService ?? throw new ArgumentNullException(nameof(customerBookingService));
            _serviceBookingExpiredDays = serviceBookingExpiredDays;
        }

        public async Task SaveCustomerVehicle(SaveCustomerVehicleRequest request, DealerConfigurationResponse dealerConfigResponse)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (dealerConfigResponse == null) throw new ArgumentNullException(nameof(dealerConfigResponse));

            var customer = MapCustomer(request);

            var customerVehicle = MapCustomerVehicle(request, customer);

            var existingCustomer = _customerVehicleDal.GetCustomer(request.CustomerNo, request.RooftopId, request.CommunityId);

            if (existingCustomer == null)
            {
                //register customer if could not find in database
                // not handlerd if cdk autoline give customer already registered.
                await _customerRegistrationService.Register(customer, customerVehicle);
            }

            var customerId = await _customerVehicleDal.SaveCustomer(customer);
            customerVehicle.CustomerId = customerId;
            await _customerVehicleDal.SaveCustomerVehicle(customerVehicle);

            var communicationMethod = GetCommunicationMethod(dealerConfigResponse.CommunicationMethod);
            var inviteCommunicationMethod = await _customerInvitationService.Invite(request, dealerConfigResponse, communicationMethod);

            var invitation = MapInvitation(request, dealerConfigResponse.DealerId, dealerConfigResponse.CommunicationMethod, inviteCommunicationMethod);
            invitation.CustomerId = customer.Id;
            await _customerVehicleDal.LogInvitationDetail(invitation);
        }

        public async Task<GetCustomerVehicleResponse> GetCustomerVehicle(int customerNo, int vehicleNo, DealerConfigurationResponse dealerConfigResponse)
        {
            if (dealerConfigResponse == null) throw new ArgumentNullException(nameof(dealerConfigResponse));
            if (customerNo <= 0) throw new ArgumentOutOfRangeException(nameof(customerNo));
            if (vehicleNo <= 0) throw new ArgumentOutOfRangeException(nameof(vehicleNo));

            var customer = _customerVehicleDal.GetCustomer(customerNo, dealerConfigResponse.RooftopId, dealerConfigResponse.CommunityId);
            if (customer == null) return null;

            var customerVehicle = _customerVehicleDal.GetCustomerVehicle(customer.Id, vehicleNo);
            if (customerVehicle == null) return null;

            if (!_customerInvitationService.ValidateLink(customerVehicle)) throw new InvitationExpiredException();

            var getCustomerVehicleResponse = _mapper.Map<Customer, GetCustomerVehicleResponse>(customer);
            _mapper.Map(customerVehicle, getCustomerVehicleResponse);

            var verifyCustomerResponse = await _customerRegistrationService.Verify(customer);

            getCustomerVehicleResponse.CdkAutolineToken = verifyCustomerResponse.CDKAutolineToken;
            getCustomerVehicleResponse.CustomerLoginId = verifyCustomerResponse.CustomerLoginId;
            return getCustomerVehicleResponse;
        }

        public async Task DismissVehicleOwnership(DismissVehicleOwnershipRequest request, DealerConfigurationResponse dealerConfigResponse)
        {
            if (dealerConfigResponse == null) throw new ArgumentNullException(nameof(dealerConfigResponse));
            if (request.CustomerNo <= 0) throw new ArgumentOutOfRangeException(nameof(request.CustomerNo));
            if (request.VehicleNo <= 0) throw new ArgumentOutOfRangeException(nameof(request.VehicleNo));
            if (request.DealerId <= 0) throw new ArgumentOutOfRangeException(nameof(request.DealerId));

            var customer = _customerVehicleDal.GetCustomer(request.CustomerNo, dealerConfigResponse.RooftopId, dealerConfigResponse.CommunityId);

            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);
            var customerVehicle = _customerVehicleDal.GetCustomerVehicle(customer.Id, request.VehicleNo);
            if (customerVehicle == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomerVehicle);

            await _customerVehicleDal.DeleteCustomerVehicle(customer.Id, customerVehicle.VehicleNo);

            await _emailService.SendDismissVehicleOwnerShipEmail(customer, request, dealerConfigResponse.EmailAddress, customerVehicle.RegistrationNo);
        }

        public async Task UpdateCustomerContact(UpdateCustomerContactRequest request, DealerConfigurationResponse dealerConfigurationResponse)
        {
            var customer = _customerVehicleDal.GetCustomer(request.CustomerNo, dealerConfigurationResponse.RooftopId, dealerConfigurationResponse.CommunityId);
            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);
            await _emailService.SendUpdateCustomerContactEmail(request, customer.CustomerNo, dealerConfigurationResponse.EmailAddress);
        }

        public async Task<CreateServiceBookingResponse> CreateServiceBooking(CreateServiceBookingRequest request,
            DealerConfigurationResponse dealerConfigurationResponse)
        {
            if (dealerConfigurationResponse.RooftopId == null) throw new ArgumentNullException(nameof(dealerConfigurationResponse.RooftopId));
            if (dealerConfigurationResponse.CommunityId == null) throw new ArgumentNullException(nameof(dealerConfigurationResponse.CommunityId));

            var customer = _customerVehicleDal.GetCustomer(request.CustomerNo, dealerConfigurationResponse.RooftopId, dealerConfigurationResponse.CommunityId);
            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);

            var customerVehicle = _customerVehicleDal.GetCustomerVehicle(customer.Id, request.VehicleNo);
            if (customerVehicle == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomerVehicle);

            var cdkCreateServiceBookingRequest = MapCreateServiceBookingRequest(customer, customerVehicle, request);
            var serviceBookingResponse = await _customerServiceBooking.CreateServiceBooking(cdkCreateServiceBookingRequest);

            var serviceBooking = MapServiceBooking(request, serviceBookingResponse);
            await _customerVehicleDal.SaveServiceBooking(serviceBooking);
            return serviceBookingResponse;
        }

        public GetCustomerVehicleResponse ExistingServiceBooking(int customerId, int vehcileNo, int dealerId)
        {
            var serviceBooking = _customerVehicleDal.GetServiceBooking(customerId, vehcileNo, dealerId);
            if (serviceBooking == null) return null;

            if (ValidateServiceBooking(serviceBooking.CreateTime))
                return new GetCustomerVehicleResponse() { ExistingBooking = serviceBooking.BookingReference };
            return null;
        }

        internal Customer MapCustomer(SaveCustomerVehicleRequest request)
        {
            var customer = _mapper.Map<SaveCustomerVehicleRequest, Customer>(request);
            return customer;
        }

        internal CustomerVehicle MapCustomerVehicle(SaveCustomerVehicleRequest request, Customer customer)
        {
            var customerVehicle = _mapper.Map<SaveCustomerVehicleRequest, CustomerVehicle>(request);
            customerVehicle.InvitationTime = DateTime.Now;
            if (customer != null) customerVehicle.Id = customer.Id;
            return customerVehicle;
        }

        internal Invitation MapInvitation(SaveCustomerVehicleRequest request, int dealerId, int method, CommunicationMethod inviteCommunicationMethod)
        {
            var invitation = _mapper.Map<SaveCustomerVehicleRequest, Invitation>(request);
            invitation.Method = method;
            invitation.Timestamp = DateTime.Now;
            invitation.DealerId = dealerId;
            switch (inviteCommunicationMethod)
            {
                case CommunicationMethod.Email:
                    invitation.ContactDetail = request.CustomerEmail;
                    break;

                case CommunicationMethod.Sms:
                    invitation.ContactDetail = request.PhoneNumber;
                    break;
            }

            return invitation;
        }

        internal CDKCreateServiceBookingRequest MapCreateServiceBookingRequest(Customer customer,
            CustomerVehicle customerVehicle, CreateServiceBookingRequest request)
        {
            var cdkCreateServiceBookingRequest = _mapper.Map<CreateServiceBookingRequest, CDKCreateServiceBookingRequest>(request);
            _mapper.Map(customer,cdkCreateServiceBookingRequest);
            _mapper.Map(customerVehicle, cdkCreateServiceBookingRequest);
            return cdkCreateServiceBookingRequest;
        }

        internal ServiceBookings MapServiceBooking(CreateServiceBookingRequest request, CreateServiceBookingResponse response)
        {
            var serviceBooking = _mapper.Map<CreateServiceBookingRequest, ServiceBookings>(request);
            serviceBooking.CreateTime = DateTime.Today;
            return _mapper.Map(response, serviceBooking);
        }

        private CommunicationMethod GetCommunicationMethod(int methodId)
        {
            return (CommunicationMethod)Enum.ToObject(typeof(CommunicationMethod), methodId);
        }

        internal bool ValidateServiceBooking(DateTime creationTime)
        {
            return (DateTime.Now - creationTime).TotalDays <= _serviceBookingExpiredDays;

        }
    }
}