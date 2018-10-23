using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contract.Response;

namespace Service.Contract
{
    public interface IEmailService
    {
        Task SendDismissVehicleOwnerShipEmail(Customer customer, DismissVehicleOwnershipRequest request,string dealerEmail, string registrationNo);
        Task SendUpdateCustomerContactEmail(UpdateCustomerContactRequest request, int customerNo, string dealerEmail);
    }
}
