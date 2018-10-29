namespace Processor.Contract
{
    public class EmailAdditionalContent
    {
        public string ContentType { get; set; }
        public string ContentName { get; set; }
        public byte[] ContentSource { get; set; }
    }
}