using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IEmailProcessor
    {
        Task<List<FileContent>> ProcessEmailContent();
    }
}