using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface ICustomerVehicleClient
    {
        Task SaveCustomerVehicle(CustomerVehicle customerVehicle);
    }
}