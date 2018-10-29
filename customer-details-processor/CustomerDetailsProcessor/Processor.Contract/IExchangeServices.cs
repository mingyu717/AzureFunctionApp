using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IExchangeServices
    {
        ExchangeService ExchangeService { get; }
        SearchFilter.IsEqualTo SearchFilter { get; }
        ItemView ItemView { get; }
    }
}
