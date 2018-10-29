using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IFileContentService
    {
        Task<List<FileContent>> GetFileContent(List<string> dealersCsvSource);
    }
}
