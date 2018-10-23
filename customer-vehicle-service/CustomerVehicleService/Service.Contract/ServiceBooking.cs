using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract
{
    public class ServiceBookings
    {
        [DataMember, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public int CustomerNo { get; set; }

        [DataMember, Required]
        public int VehicleNo { get; set; }

        [DataMember, Required]
        public int DealerId { get; set; }

        [DataMember, Required]
        public string BookingReference { get; set; }

        [DataMember, Required]
        public DateTime CreateTime { get; set; }
    }
}
