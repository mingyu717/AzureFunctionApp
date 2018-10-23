using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Contract
{
    public class DealerConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DealerId { get; set; }

        [Column("Name")]
        [StringLength(50)]
        public string DealerName { get; set; }

        [Required]
        [StringLength(50)]
        public string RooftopId { get; set; }

        [Required]
        [StringLength(50)]
        public string CommunityId { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        public int CommunicationMethod { get; set; }

        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; }

        [Required]
        public string EmailContent { get; set; }

        [Required]
        public string SmsContent { get; set; }

        [Required]
        public string EmailSubject { get; set; }

        [Required]
        public int CsvSource { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [StringLength(50)]
        public string AppThemeName { get; set; }
        
        public bool ShowTransportations { get; set; }

        public bool ShowAdvisors { get; set; }

        public bool ShowPrice { get; set; }

        public int MinimumFreeCapacity { get; set; }
    }
}