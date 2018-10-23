using Service.Contract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Service.Implementation
{
    /// <summary>
    /// Validate Request class is responsible for validating request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidateRequest : IValidateRequest
    {
        /// <summary>
        /// Used to validate input based upon given annotation(s)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validateObject"></param>
        /// <returns></returns>
        public IEnumerable<string> ValidateRequestData<T>(T validateObject) where T : class
        {
            if (validateObject == null) return new List<string> { Constants.ExceptionMessages.InvalidRequest };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(validateObject, null, null);
            return !Validator.TryValidateObject(validateObject, context, results, true)
                ? results.Select(x => x.ErrorMessage)
                : null;
        }

        /// <summary>
        /// Validate dealer id
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public string ValidateDealerIdRequest(int dealerId)
        {
            return dealerId == default(int) ? Constants.ExceptionMessages.DealerIdRequired : string.Empty;
        }

        /// <summary>
        /// Validate dealer rooftopid and community id
        /// </summary>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public List<string> ValidateDealerRoofTopIdAndCommunityIdRequest(string roofTopId, string communityId)
        {
            List<string> error = new List<string>();
            if (roofTopId == null) error.Add(Constants.ExceptionMessages.RoofTopIdRequired);
            if (communityId == null) error.Add(Constants.ExceptionMessages.CommunityIdRequired);
            return error;
        }
    }
}
