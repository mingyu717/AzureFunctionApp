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
            if (validateObject == null) return new List<string> {Constants.ExceptionMessages.InvalidRequest};
            var results = new List<ValidationResult>();
            var context = new ValidationContext(validateObject, null, null);
            return !Validator.TryValidateObject(validateObject, context, results, true)
                ? results.Select(x => x.ErrorMessage)
                : null;
        }
    }
}