using System;

namespace Service.Contract.Exceptions
{
    public class CustomerNotRegisterException : Exception
    {
        public CustomerNotRegisterException(string message) : base(message)
        {
        }
    }
}