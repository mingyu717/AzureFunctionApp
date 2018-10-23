using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract.Models
{
    public class ServiceResponse
    {
        public string Name { get; set; }
        public string ServiceCode { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string ServiceTime { get; set; }
        public string Price { get; set; }
    }
}
