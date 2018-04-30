using Payvision.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Interfaces
{
    public interface IOrderService
    {
        void EnsureOrderFieldsAreValid(string orderId, string dealId, string email, string street, string city,
            string state);
        void EnsureOrderHasAllMandatoryFields(string[] order);
        void EnsureFIeldIsFilled(string field, string errorMessage);
        void EnsureFieldIsNumeric(string field, string errorMessage);

        string NormalizeStreetAddress(string street);
        string NormalizeStateAddress(string address);
        string NormalizeEmailAddress(string email);

        IList<Order> ReadOrders(string path);
    }
}
