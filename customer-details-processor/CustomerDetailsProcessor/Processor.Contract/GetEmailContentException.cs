namespace Processor.Contract
{
    public class GetEmailContentException : ContentException
    {
        public GetEmailContentException(string message, string stackTrace): base(message, stackTrace)
        {

        }        
    }
}