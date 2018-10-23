using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contract;
using Service.Contract.Response;

namespace Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IEmailGatewayClient _emailGatewayClient;
        private readonly string _serviceBookingEmail;

        public EmailService(IEmailGatewayClient emailGatewayClient, string serviceBookingEmail)
        {
            _emailGatewayClient = emailGatewayClient ?? throw new ArgumentNullException(nameof(emailGatewayClient));
            _serviceBookingEmail = serviceBookingEmail ?? throw new ArgumentNullException(nameof(serviceBookingEmail));
        }

        public async Task SendDismissVehicleOwnerShipEmail(Customer customer, DismissVehicleOwnershipRequest request, string dealerEmail, string registrationNo)
        {
            var subject = $"{customer.FirstName} {customer.Surname} does not own the vehicle {registrationNo} anymore";
            var contentBody = GetDismissVehicleOwnerShipText(customer, request, registrationNo);
            await _emailGatewayClient.SendHtmlEmail(_serviceBookingEmail, dealerEmail, subject, contentBody);
        }

        public async Task SendUpdateCustomerContactEmail(UpdateCustomerContactRequest request, int customerNo, string dealerEmail)
        {
            var subject = $"{request.FirstName} {request.SurName} has updated the contact information.";
            var contentBody = GetUpdateCustomerEmailText(request,customerNo);
            await _emailGatewayClient.SendHtmlEmail(_serviceBookingEmail, dealerEmail, subject, contentBody);
        }

        internal string GetDismissVehicleOwnerShipText(Customer customer, DismissVehicleOwnershipRequest request, string registrationNo)
        {
            return
                $"Hi,<br/><br/>We would like to inform you that the customer {customer.FirstName} {customer.Surname}," +
                $" CustomerNo {customer.CustomerNo}, does not own the vehicle with the registration number" +
                $" {registrationNo} and VehicleNo {request.VehicleNo} anymore. Please update your records." +
                $"<br/><br/>Best Regards<br/><br/>Service Booking Team";
        }

        internal string GetUpdateCustomerEmailText(UpdateCustomerContactRequest request, int customerNo)
        {
            return
                $"Hi,<br/><br/>We would like to inform you that { request.FirstName}, { customerNo}," +
                $" have updated their contact information.Please update your record. Here is the last contact information:" +
                $"<br/><br/>First Name: { request.FirstName}" +
                $"<br/><br/>Last Name: { request.SurName}" +
                $"<br/><br/>Email: { request.CustomerEmail}" +
                $"<br/><br/>Phone Number: { request.PhoneNumber}" +
                $"<br/><br/>Additional Comments: { request.AdditionalComments}" +
                $"<br/><br/>Best Regards<br/><br/>Service Booking Team";
        }
    }
}
