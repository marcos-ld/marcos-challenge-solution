using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Model
{
    public class FraudResult
    {
        public string File { get; set; }

        public int OrderId { get; set; }

        public bool IsFraudulent { get; set; }
    }
}
