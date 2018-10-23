using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Request
{
    public class DealerConfigurationUpdateRequest
    {
        [Required]
        public string DealerName { get; set; }

        [Required]
        public string RooftopId { get; set; }

        [Required]
        public string CommunityId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string CommunicationMethodName { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string EmailContent { get; set; }

        [Required]
        public string EmailSubject { get; set; }

        [Required]
        public string SmsContent { get; set; }

        [Required]
        public string CsvSourceName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string AppThemeName { get; set; }
        
        public bool ShowTransportations { get; set; } = true;

        public bool ShowAdvisors { get; set; } = true;

        public bool ShowPrice { get; set; } = true;

        public int MinimumFreeCapacity { get; set; } = 0;
    }
}