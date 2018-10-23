using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract
{
    [DataContract]
    public class Invitation
    {
        [DataMember, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember]
        public int CustomerId { get; set; }

        [DataMember, Required]
        public int DealerId { get; set; }

        [DataMember, Required]
        public int Method { get; set; }

        [DataMember, StringLength(150)]
        public string ContactDetail { get; set; }

        [DataMember, Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}