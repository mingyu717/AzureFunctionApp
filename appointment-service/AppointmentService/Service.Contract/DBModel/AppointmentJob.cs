using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract.DBModel
{
    public class AppointmentJob
    {
        [DataMember, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public int AppointmentId { get; set; }

        [DataMember, Required]
        public int JobId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }
    }
}
