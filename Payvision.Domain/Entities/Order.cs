using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; }

        public int DealId { get; }

        public string Email { get; }

        public string Street { get; }

        public string City { get; }

        public string State { get; }

        public string ZipCode { get; }

        public string CreditCard { get; }

        public Order(int orderId, int dealId, string email, string street, string city, string state, string zipCode, string creditCard)
        {
            OrderId = orderId;
            DealId = dealId;
            Email = email;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            CreditCard = creditCard;
        }

        public void Normalize(Interfaces.IOrderService orderService)
        {
            orderService.NormalizeEmailAddress(Email);
            orderService.NormalizeStreetAddress(City);
            orderService.NormalizeStateAddress(State);
        }
    }
}
