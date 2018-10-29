using System;

namespace Processor.Implementation
{
    /// <summary>
    /// Custom Exception
    /// </summary>
    public class CsvInvalidHeaderException : Exception
    {
        public CsvInvalidHeaderException(string message) : base(message)
        {
        }
    }
}