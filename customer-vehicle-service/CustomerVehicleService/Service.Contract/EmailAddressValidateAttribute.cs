using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract
{
    public class EmailAddressValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                if (value!= null && value as string != string.Empty)
                {
                    var addr = new System.Net.Mail.MailAddress(value.ToString());
                    return string.Equals(addr.Address, value.ToString(), StringComparison.OrdinalIgnoreCase);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
