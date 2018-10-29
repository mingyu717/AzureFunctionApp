using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface ICsvProcessor
    {
        Task<List<CustomerVehicle>> ProcessCSVFile(byte[] content);
    }
}
