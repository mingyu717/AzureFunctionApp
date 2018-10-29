using System;

namespace Processor.Contract.Exceptions
{
    public class DealerNotFoundException : Exception
    {
        public DealerNotFoundException()
        {
        }

        public DealerNotFoundException(string message) : base(message)
        {
        }
    }
}