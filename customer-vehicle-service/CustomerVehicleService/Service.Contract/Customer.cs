using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract
{
    [DataContract]
    public class Customer
    {
        [DataMember, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public int CustomerNo { get; set; }

        [DataMember, StringLength(150)]
        public string CustomerEmail { get; set; }

        [DataMember, Required, StringLength(50)]
        public string FirstName { get; set; }

        [DataMember, Required, StringLength(50)]
        public string Surname { get; set; }

        [DataMember, StringLength(50)]
        public string PhoneNumber { get; set; }
        
        [DataMember, Required, StringLength(50)]
        public string CommunityId { get; set; }

        [DataMember, Required, StringLength(50)]
        public string RooftopId { get; set; }
        
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<CustomerVehicle> CustomerVehicles { get; set; }
    }
}