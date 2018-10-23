using System.Collections.Generic;

namespace Service.Contract
{
    public interface IValidateRequest
    {
        IEnumerable<string> ValidateRequestData<T>(T validateObject) where T : class;
        string ValidateDealerIdRequest(int dealerId);
        List<string> ValidateDealerRoofTopIdAndCommunityIdRequest(string roofTopId, string communityId);
    }
}
