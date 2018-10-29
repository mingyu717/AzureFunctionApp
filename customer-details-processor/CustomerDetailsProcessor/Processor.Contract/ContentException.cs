using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public class ContentException: Exception
    {
        public ContentException(string message, string stackTrace) : base(message)
        {
            StackTrace = stackTrace;
        }

        public override string StackTrace { get; }
    }
}
