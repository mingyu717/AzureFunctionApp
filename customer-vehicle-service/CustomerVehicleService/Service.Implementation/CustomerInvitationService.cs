using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using PhoneNumbers;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Response;

namespace Service.Implementation
{
    public class CustomerInvitationService : ICustomerInvitationService
    {
        private readonly string _serviceBookingAppUrl;
        private readonly int _invitationExpiredDays;
        private readonly string _invitationFromPhone;
        private readonly ISMSGatewayClient _smsGatewayClient;
        private readonly IEmailGatewayClient _emailGatewayClient;
        private readonly TelemetryClient _telemetryClient;
        private readonly IDealerConfigurationService _dealerConfigurationService;
        private readonly string _serviceBookingUrlPlaceholder;
        private readonly string _registrationNoPlaceholder;

        public CustomerInvitationService(string serviceBookingAppUrl, int invitationExpiredDays,
            string invitationFromPhone,
            ISMSGatewayClient smsGatewayClient, IEmailGatewayClient emailGatewayClient,
            TelemetryClient telemetryClient, IDealerConfigurationService dealerConfigurationService,
            string serviceBookingUrlPlaceholder, string registrationNoPlaceholder)
        {
            if (invitationExpiredDays <= 0) throw new ArgumentOutOfRangeException(nameof(invitationExpiredDays));
            _serviceBookingAppUrl = serviceBookingAppUrl ?? throw new ArgumentNullException(nameof(serviceBookingAppUrl));
            _invitationExpiredDays = invitationExpiredDays;
            _smsGatewayClient = smsGatewayClient ?? throw new ArgumentNullException(nameof(smsGatewayClient));
            _invitationFromPhone = invitationFromPhone ?? throw new ArgumentNullException(nameof(invitationFromPhone));
            _emailGatewayClient = emailGatewayClient ?? throw new ArgumentNullException(nameof(emailGatewayClient));
            _telemetryClient = telemetryClient;
            _dealerConfigurationService = dealerConfigurationService ?? throw new ArgumentNullException(nameof(dealerConfigurationService));
            _serviceBookingUrlPlaceholder = serviceBookingUrlPlaceholder ?? throw new ArgumentNullException(nameof(serviceBookingUrlPlaceholder));
            _registrationNoPlaceholder = registrationNoPlaceholder ?? throw new ArgumentNullException(nameof(registrationNoPlaceholder));
        }

        public async Task<CommunicationMethod> Invite(SaveCustomerVehicleRequest saveCustomerVehicleRequest, DealerConfigurationResponse dealerConfigResponse, CommunicationMethod method)
        {
            if (saveCustomerVehicleRequest == null) throw new ArgumentNullException(nameof(saveCustomerVehicleRequest));
            return await SendInvite(saveCustomerVehicleRequest, dealerConfigResponse, method);
        }

        internal string FormatInternationalMobileNumber(string phoneNumber)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var parsedPhoneNumber = phoneUtil.Parse(phoneNumber, "NZ");
            return phoneUtil.Format(parsedPhoneNumber, PhoneNumberFormat.INTERNATIONAL);
        }

        public bool ValidateLink(CustomerVehicle customerVehicle)
        {
            return customerVehicle.InvitationTime == null ||
                   (DateTime.Now - customerVehicle.InvitationTime.Value).TotalDays <= _invitationExpiredDays;
        }

        internal string GetInvitationText(int dealerId, int customerNo, int vehicleNo, string registrationNo, string content)
        {
            var link =
                $"{_serviceBookingAppUrl}?d={dealerId}&cno={customerNo}&vno={vehicleNo}";

            return content.Replace(_serviceBookingUrlPlaceholder, link).Replace(_registrationNoPlaceholder, registrationNo);
        }

        internal async Task<CommunicationMethod> SendInvite(SaveCustomerVehicleRequest request,
            DealerConfigurationResponse dealerConfigResponse, CommunicationMethod method)
        {
            var dealerInvitationResponse = await _dealerConfigurationService.GetDealerInvitationContent(dealerConfigResponse.RooftopId, dealerConfigResponse.CommunityId);
            if (dealerInvitationResponse == null) throw new DealerInvitationContentException();

            switch (method)
            {
                case CommunicationMethod.Email: // Send invitation via email.
                    return await SendEmailWithOtherOption(request, dealerConfigResponse, dealerInvitationResponse);
                case CommunicationMethod.SmsOrEmail:
                {
                    if (!string.IsNullOrEmpty(request.PhoneNumber))
                        return await SendSmsWithOtherOption(request, dealerConfigResponse, dealerInvitationResponse, true);
                    return await SendEmailWithOtherOption(request, dealerConfigResponse, dealerInvitationResponse);
                }
                case CommunicationMethod.EmailOrSms:
                    return await SendEmailWithOtherOption(request, dealerConfigResponse, dealerInvitationResponse, true);
                case CommunicationMethod.Sms: // Send invitation via sms.
                    return await SendSmsWithOtherOption(request, dealerConfigResponse, dealerInvitationResponse);
                default:
                    return CommunicationMethod.None;
            }
        }

        internal async Task<(bool, Exception)> SendSmsOrEmail(SaveCustomerVehicleRequest request,
            DealerConfigurationResponse dealerResponse, DealerInvitationContentResponse dealerInvitationResponse, bool sendEmail = false)
        {
            if (sendEmail)
                return await _emailGatewayClient.SendHtmlEmail(dealerResponse.EmailAddress, request.CustomerEmail, dealerInvitationResponse.EmailSubject,
                    GetInvitationText(dealerResponse.DealerId, request.CustomerNo, request.VehicleNo, request.RegistrationNo, dealerInvitationResponse.EmailContent));

            var phoneNumber = FormatInternationalMobileNumber(request.PhoneNumber);
            return _smsGatewayClient.SendMessage(_invitationFromPhone, phoneNumber,
                GetInvitationText(dealerResponse.DealerId, request.CustomerNo, request.VehicleNo, request.RegistrationNo, dealerInvitationResponse.SmsContent));
        }

        internal async Task<CommunicationMethod> SendEmailWithOtherOption(SaveCustomerVehicleRequest request,
            DealerConfigurationResponse dealerResponse, DealerInvitationContentResponse dealerInvitationResponse, bool sendSmsAsSecondOption = false)
        {
            (bool emailResponse, Exception ex) = await SendSmsOrEmail(request, dealerResponse, dealerInvitationResponse, true);
            if (!emailResponse)
            {
                if (sendSmsAsSecondOption)
                {
                    _telemetryClient?.TrackException(ex);
                    (bool smsResponse, Exception smsEx) = await SendSmsOrEmail(request, dealerResponse, dealerInvitationResponse);
                    if (!smsResponse) throw smsEx;
                    return CommunicationMethod.Sms;
                }

                throw ex;
            }

            return CommunicationMethod.Email;
        }

        internal async Task<CommunicationMethod> SendSmsWithOtherOption(SaveCustomerVehicleRequest request,
            DealerConfigurationResponse dealerResponse, DealerInvitationContentResponse dealerInvitationResponse, bool sendEmailAsSecondOption = false)
        {
            (bool smsResponse, Exception ex) = await SendSmsOrEmail(request, dealerResponse, dealerInvitationResponse);
            if (!smsResponse)
            {
                if (sendEmailAsSecondOption)
                {
                    _telemetryClient?.TrackException(ex);
                    (bool emailResponse, Exception emailEx) = await SendSmsOrEmail(request, dealerResponse, dealerInvitationResponse, true);
                    if (!emailResponse) throw emailEx;
                    return CommunicationMethod.Email;
                }

                throw ex;
            }

            return CommunicationMethod.Sms;
        }
    }
}