using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Processor.Contract;

namespace Processor.Implementation
{
    public class CustomerVehicleClient : ICustomerVehicleClient
    {
        private readonly IRestfulClient _restfulClient;
        private readonly IMapper _mapper;
        private readonly string SaveCustomerVehicleUrl = "SaveCustomerVehicle";

        public CustomerVehicleClient(IRestfulClient restfulClient, IMapper mapper)
        {
            _restfulClient = restfulClient ??
                             throw new ArgumentNullException(nameof(restfulClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task SaveCustomerVehicle(CustomerVehicle customerVehicle)
        {
            if (customerVehicle == null) throw new ArgumentNullException(nameof(customerVehicle));
            var response = await _restfulClient.PostAsync(SaveCustomerVehicleUrl, GetSaveCustomerVehicleRequest(customerVehicle));            
        }

        internal SaveCustomerVehicleRequest GetSaveCustomerVehicleRequest(CustomerVehicle customerVehicle)
        {
            return _mapper.Map<CustomerVehicle, SaveCustomerVehicleRequest>(customerVehicle);
        }
    }
}