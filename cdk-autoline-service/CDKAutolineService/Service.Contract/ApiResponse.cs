using System;
using System.Collections.Generic;

namespace Service.Contract
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string ResponseCode { get; set; }
        public object Result { get; set; }       
        public List<String> Errors { get; private set; }

        public void AddErrors(String error)
        {
            this.Errors = new List<string>() { error };
        }

        public void AddErrors(List<String> errors)
        {
            this.Errors = errors;
        }
    }
    
}
