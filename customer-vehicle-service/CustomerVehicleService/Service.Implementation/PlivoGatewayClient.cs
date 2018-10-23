using System;
using System.Collections.Generic;
using Plivo;
using Service.Contract;

namespace Service.Implementation
{
    public class PlivoGatewayClient : ISMSGatewayClient
    {
        private readonly string _pilvoAuthId;
        private readonly string _pilvoAuthToken;

        public PlivoGatewayClient(string pilvoAuthId, string pilvoAuthToken)
        {
            _pilvoAuthId = pilvoAuthId ?? throw new ArgumentNullException(nameof(pilvoAuthId));
            _pilvoAuthToken = pilvoAuthToken ?? throw new ArgumentNullException(nameof(pilvoAuthToken));
        }

        public (bool, Exception) SendMessage(string fromNumber, string toNumber, string text)
        {
            try
            {
                var plivoApi = new PlivoApi(_pilvoAuthId, _pilvoAuthToken);
                plivoApi.Message.Create(
                    dst: new List<String> { toNumber },
                    src: fromNumber,
                    text: text
                );
                return (true, null);
            }
            catch(Exception ex)
            {
                return (false, ex);
            }
        }
    }
}