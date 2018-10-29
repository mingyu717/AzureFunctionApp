using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IAzureFileProcessor
    {
        Task<List<FileContent>> ProcessAzureFilesContent();
    }
}
