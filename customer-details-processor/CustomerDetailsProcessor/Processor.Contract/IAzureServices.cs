using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IAzureServices
    {
        Task<List<FileContent>> GetCloudContent();
    }
}
