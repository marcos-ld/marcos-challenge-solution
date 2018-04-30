using Payvision.Domain.Entities;
using Payvision.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Interfaces
{
    public interface IFraudService
    {
        void EnsureFilePathIsValid(string file);

        FraudRequestValidation IsValidRequest(FraudRequest request);

        bool LookForCreditCardFraudByEmail(Order order, Order orderToCompare);
        bool LookForCreditCardFraudByAddress(Order order, Order orderToCompare);
    }
}
