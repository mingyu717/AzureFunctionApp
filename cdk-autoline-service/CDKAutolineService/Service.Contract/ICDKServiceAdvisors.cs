﻿using Service.Contract.Models;
using Service.Contract.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICDKServiceAdvisors
    {
        Task<GetServiceAdvisorsResponse> GetServiceAdvisors(GetServiceAdvisorsRequest request);
    }
}
