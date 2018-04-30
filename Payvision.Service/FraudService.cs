using Payvision.Domain.Entities;
using Payvision.Domain.Interfaces;
using Payvision.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Payvision.Service
{
    public class FraudService : BaseService, IFraudService
    {
        public FraudService(ILoggerService loggerService) : base(loggerService)
        {
        }

        public void EnsureFilePathIsValid(string file)
        {
            if (!File.Exists(file))
                throw new ArgumentException("File does not exist");

            if(!file.EndsWith(".txt"))
                throw new ArgumentException("File extension is not supported. .txt allowed only");
        }

        public FraudRequestValidation IsValidRequest(FraudRequest request)
        {
            if (string.IsNullOrEmpty(request.Directory) || !Directory.Exists(request.Directory))
                return new FraudRequestValidation(false, "Invalid Directory");

            if (  (string.IsNullOrEmpty(request.SearchPattern) && string.IsNullOrEmpty(request.FileName))
                || (!string.IsNullOrEmpty(request.SearchPattern) && !request.SearchPattern.EndsWith(".txt"))
                || (!string.IsNullOrEmpty(request.FileName) && !request.FileName.EndsWith(".txt")))
                return new FraudRequestValidation(false,"Invalid Search Pattern Extension, and/or File. *.txt Allowed only");

            return new FraudRequestValidation(true);
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

        public bool LookForCreditCardFraudByAddress(Order order, Order orderToCompare)
        {
            return (order.DealId == orderToCompare.DealId
                        && order.State == orderToCompare.State
                        && order.ZipCode == orderToCompare.ZipCode
                        && order.Street == orderToCompare.Street
                        && order.City == orderToCompare.City
                        && order.CreditCard != orderToCompare.CreditCard);
        }

        public bool LookForCreditCardFraudByEmail(Order order, Order orderToCompare)
        {
            return (order.DealId == orderToCompare.DealId
                        && order.Email == orderToCompare.Email
                        && order.CreditCard != orderToCompare.CreditCard);
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
                catch(Exception ex)
                {
                    Logger.Error($"Problem reading and converting Order {items[0]}", ex);
                }
            }

            return orders;
        }
    }
}
