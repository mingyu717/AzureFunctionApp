using System;

namespace Processor.Contract.Exceptions
{
    public class PhoneOrEmailNullException : Exception
    {
        public PhoneOrEmailNullException(string message) : base(message)
        {
        }
    }
}
