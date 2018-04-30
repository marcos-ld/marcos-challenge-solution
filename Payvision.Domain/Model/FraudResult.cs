using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Model
{
    public class FraudResult
    {
        public string File { get; }

        public int OrderId { get; }

        public bool IsFraudulent { get; }

        public FraudResult(int orderId, bool isFraudulent)
        {
            OrderId = orderId;
            IsFraudulent = isFraudulent;
        }

        public FraudResult(string file, int orderId, bool isFraudulent)
        {
            File = file;
            OrderId = orderId;
            IsFraudulent = isFraudulent;
        }
    }
}
