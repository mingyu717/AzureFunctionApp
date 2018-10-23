using System;

namespace Service.Contract
{
    public interface ISMSGatewayClient
    {
        (bool, Exception) SendMessage(string fromNumber, string toNumber, string text);
    }
}