using System.ComponentModel.DataAnnotations;

namespace Service.Contract
{
    public class DismissVehicleOwnershipRequest
    {
        [Required]
        public int DealerId { get; set; }

        [Required]
        public int CustomerNo { get; set; }

        [Required]
        public int VehicleNo { get; set; }
    }
}
