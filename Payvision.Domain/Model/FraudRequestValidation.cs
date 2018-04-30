using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Model
{
    public class FraudRequestValidation
    {
        public bool Success { get; }
        public string Message { get; }

        public FraudRequestValidation(bool success)
        {
            Success = success;
        }

        public FraudRequestValidation(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
