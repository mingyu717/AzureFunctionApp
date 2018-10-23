using System.Collections.Generic;

namespace Service.Contract.Response
{
    public class GetServiceAdvisorsResponse
    {
        public Results Results { get; set; }
        public string PreferredSA { get; set; }
        public string PreferredSAName { get; set; }
        public bool PreferredSAAvailToday { get; set; }
        public ErrorResponse Result { get; set; }
    }
    public class Results
    {
        public List<AdvisorData> AdvisorData { get; set; }
    }
}
