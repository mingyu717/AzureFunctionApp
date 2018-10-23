using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Service.Contract.DbModels
{
    public class DealerCDKConfiguration
    {
        [DataMember, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember, Required]
        public string CommunityId { get; set; }

        [DataMember, Required]
        public string RoofTopId { get; set; }

        [DataMember, Required]
        public string PartnerId { get; set; }

        [DataMember, Required]
        public string PartnerKey { get; set; }

        [DataMember, Required]
        public string PartnerVersion { get; set; }
    }
}
