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
    }
}
