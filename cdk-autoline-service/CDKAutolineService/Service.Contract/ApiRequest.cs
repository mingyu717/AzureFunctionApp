using System.Collections.Generic;

namespace Service.Contract
{
    public class ApiRequest
    {
        private Dictionary<string, string> _headers;

        public string Url { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public string Accept { get; set; }
        public Dictionary<string, string> Headers
        {
            get
            {
                if(_headers == null)
                {
                    return _headers = new Dictionary<string, string>();
                }
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }
    }
}
