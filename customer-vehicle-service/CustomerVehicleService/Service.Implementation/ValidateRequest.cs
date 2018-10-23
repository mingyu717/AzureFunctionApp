using Service.Contract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Service.Contract.Exceptions;

namespace Service.Implementation
{
    public class ValidateRequest : IValidateRequest
    {
        public IEnumerable<string> ValidateRequestData<T>(T validateObject) where T : class
        {
            if (validateObject == null) return new List<string> { ExceptionMessages.InvalidRequest };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(validateObject, null, null);
            return !Validator.TryValidateObject(validateObject, context, results, true)
                ? results.Select(x => x.ErrorMessage)
                : null;
        }
    }
}
