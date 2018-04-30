using Payvision.Domain.Entities;
using Payvision.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Service
{
    public class OrderService : BaseService, IOrderService
    {
        public OrderService(ILoggerService loggerService) : base(loggerService)
        {
        }

        public void EnsureFieldIsNumeric(string field, string errorMessage)
        {
            if (!Int32.TryParse(field, out int i))
                throw new ArgumentException(errorMessage);
        }

        public void EnsureFIeldIsFilled(string field, string errorMessage)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException(errorMessage);
        }

        public void EnsureOrderHasAllMandatoryFields(string[] order)
        {
            if (order.Length < 6)
                throw new ArgumentException("Order does not contain the right number of fields");
        }

        public void EnsureOrderFieldsAreValid(string orderId, string dealId, string email, string street, string city,
            string state)
        {
            EnsureFIeldIsFilled(orderId, "OrderId must be informed.");
            EnsureFieldIsNumeric(orderId, "OrderId must be a number.");

            EnsureFIeldIsFilled(dealId, "DealId must be informed.");
            EnsureFieldIsNumeric(dealId, "DealId must be a number.");

            EnsureFIeldIsFilled(email, "Email must be informed.");

            EnsureFIeldIsFilled(street, "Street must be informed.");
            EnsureFIeldIsFilled(city, "City must be informed.");
            EnsureFIeldIsFilled(state, "State must be informed.");

            //Not checking credit card number, and zip code, because there are no implicity function that would cause an exception
            //i.e. Parsing a 123asidj into an int, or using .ToLower() in a field with null value.
        }

        public string NormalizeEmailAddress(string email)
        {
            var aux = email?.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            if (aux == null || aux.Length <= 1)
                return email;

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }

        public string NormalizeStateAddress(string state)
        {
            return state?.Replace("il", "illinois")
                ?.Replace("ca", "california")
                ?.Replace("ny", "new york");
        }

        public string NormalizeStreetAddress(string street)
        {
            return street
                ?.Replace("st.", "street")
                ?.Replace("rd.", "road");
        }

        public IList<Order> ReadOrders(string path)
        {
            var orders = new List<Order>();
            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var items = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    EnsureOrderHasAllMandatoryFields(items);

                    EnsureOrderFieldsAreValid(items[0], items[1], items[2], items[3], items[4], items[5]);

                    orders.Add(new Order
                    (
                        orderId: int.Parse(items[0]),
                        dealId: int.Parse(items[1]),
                        email: items[2].ToLower(),
                        street: items[3].ToLower(),
                        city: items[4].ToLower(),
                        state: items[5].ToLower(),
                        zipCode: items[6],
                        creditCard: items[7]
                    ));
                }
                catch (Exception ex)
                {
                    Logger.Error($"Problem reading and converting Order {items[0]}", ex);
                }
            }

            return orders;
        }
    }
}
