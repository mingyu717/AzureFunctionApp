using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public class UpdateCustomerContactRequest
    {
        [Required]
        public int DealerId { get; set; }

        [Required]
        public int CustomerNo { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SurName { get; set; }

        public string CustomerEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalComments { get; set; }
    }
}
