using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(List<string> validateMessage)
        {
            ValidateMessages = validateMessage;
        }

        public List<string> ValidateMessages { get; set; }
    }
}
