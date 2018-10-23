using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract.DBModel
{
    [DataContract]
    public class Appointment
    {
        [DataMember, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public int DealerId { get; set; }

        [DataMember, Required, StringLength(50)]
        public string CustomerFirstName { get; set; }

        [DataMember, Required, StringLength(50)]
        public string CustomerSurName { get; set; }

        [DataMember, Required, StringLength(150)]
        public string EmailAddress { get; set; }

        [DataMember, Required, StringLength(50)]
        public string Mobile { get; set; }

        [DataMember, Required, StringLength(10)]
        public string VehicleRegistrationNumber { get; set; }

        [DataMember, Required]
        public DateTime JobDate { get; set; }

        [DataMember]
        public string TransportMethod { get; set; }

        public string DropOffTime { get; set; }
    }
}
