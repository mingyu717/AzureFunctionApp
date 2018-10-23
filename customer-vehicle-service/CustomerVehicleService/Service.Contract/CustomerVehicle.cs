using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract
{
    [DataContract]
    public class CustomerVehicle
    {
        [DataMember, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public int CustomerId { get; set; }

        [DataMember, Required]
        public int VehicleNo { get; set; }

        [DataMember, Required, StringLength(10)]
        public string RegistrationNo { get; set; }

        [DataMember, Required, StringLength(50)]
        public string VinNumber { get; set; }

        [DataMember, Required, StringLength(5)]
        public string MakeCode { get; set; }

        [DataMember, Required, StringLength(20)]
        public string ModelCode { get; set; }

        [DataMember, StringLength(10)]
        public string ModelYear { get; set; }

        [DataMember, StringLength(100)]
        public string ModelDescription { get; set; }

        [DataMember, StringLength(50)]
        public string LastServiceDate { get; set; }

        [DataMember, StringLength(50)]
        public string NextServiceDate { get; set; }

        [DataMember]
        public int LastKnownMileage { get; set; }

        [DataMember]
        public int NextServiceMileage { get; set; }

        [DataMember, Required, StringLength(50)]
        public string VariantCode { get; set; }

        [Required]
        public bool IsProcessed { get; set; }

        public DateTime? InvitationTime { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}