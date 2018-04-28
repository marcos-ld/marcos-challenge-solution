using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Model
{
    public class FraudRequestValidation
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
