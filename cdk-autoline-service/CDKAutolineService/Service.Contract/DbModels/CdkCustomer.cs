using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Contract.DbModels
{
    public class CdkCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CommunityId { get; set; }

        [Required]
        public int CustomerNo { get; set; }

        [Required]
        [StringLength(150)]
        public string CustomerLoginId { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public Guid? Token { get; set; }
    }
}