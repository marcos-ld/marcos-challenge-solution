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
        void EnsureOrderFieldsAreValid(string orderId, string dealId, string email, string street, string city,
            string state);
        void EnsureOrderHasAllMandatoryFields(string[] order);
        void EnsureFIeldIsFilled(string field, string errorMessage);
        void EnsureFieldIsNumeric(string field, string errorMessage);

        FraudRequestValidation IsValidRequest(FraudRequest request);

        bool LookForCreditCardFraudByEmail(Order order, Order orderToCompare);
        bool LookForCreditCardFraudByAddress(Order order, Order orderToCompare);

        string NormalizeStreetAddress(string street);
        string NormalizeStateAddress(string address);
        string NormalizeEmailAddress(string email);

        IList<Order> ReadOrders(string path);
    }
}
