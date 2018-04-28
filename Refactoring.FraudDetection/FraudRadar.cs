// -----------------------------------------------------------------------
// <copyright file="FraudRadar.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Refactoring.FraudDetection
{
    using Payvision.Domain.Interfaces;
    using Autofac;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Linq;
    using Payvision.Domain.Model;
    using Payvision.Domain.Entities;

    public class FraudRadar
    {
        private readonly IFraudService _fraudService;
        private readonly ILoggerService _loggerService;

        public FraudRadar()
        {
            _fraudService = Common.Ioc.ApplicationContainer.Resolve<IFraudService>();
            _loggerService = Common.Ioc.ApplicationContainer.Resolve<ILoggerService>();
        }

        public FraudRadar(IFraudService fraudService, ILoggerService loggerService)
        {
            _fraudService = fraudService;
            _loggerService = loggerService;
        }

        public IEnumerable<FraudResult> Check(FraudRequest request)
        {
            try
            {
                var result = new List<FraudResult>();

                // CHECK IF FILE EXISTS AND IF THE EXTESION IS VALID
                ValidateRequest(request);

                string searchPattern = string.IsNullOrEmpty(request.FileName) ? request.SearchPattern : request.FileName;

                foreach (var file in Directory.EnumerateFiles(request.Directory, searchPattern))
                    result.AddRange(Check(file));

                return result;
            }
            catch(Exception ex)
            {
                _loggerService.Error("Problem on Checking for Frauds", ex);
                throw ex;
            }
        }

        private IEnumerable<FraudResult> Check(string filePath)
        {
            try
            {
                _fraudService.EnsureFilePathIsValid(filePath);

                // READ FRAUD LINES
                var orders = _fraudService.ReadOrders(filePath);

                // NORMALIZE
                NormalizeOrders(orders);

                // CHECK FRAUD
                return RunAnalysis(orders);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Problem on Checking for Frauds on file '${filePath}'", ex);
                throw ex;
            }
        }

        public void ValidateRequest(FraudRequest request)
        {
            var requestValidation = _fraudService.IsValidRequest(request);

            if (!requestValidation.Success)
                throw new ArgumentException(requestValidation.Message);
        }

        public void NormalizeOrders(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                _fraudService.NormalizeEmailAddress(order.Email);
                _fraudService.NormalizeStreetAddress(order.City);
                _fraudService.NormalizeStateAddress(order.State);
            }
        }

        public IList<FraudResult> RunAnalysis(IList<Order> orders)
        {
            var fraudResults = new List<FraudResult>();
            
            for (int i = 0; i < orders.Count; i++)
            {
                var current = orders[i];

                for (int j = i + 1; j < orders.Count; j++)
                {
                    var orderToCompare = orders[j];

                    if (_fraudService.LookForCreditCardFraudByEmail(current, orderToCompare)
                        || _fraudService.LookForCreditCardFraudByAddress(current, orderToCompare))
                        fraudResults.Add(new FraudResult { IsFraudulent = true, OrderId = orderToCompare.OrderId });
                }
            }

            return fraudResults;
        }
    }
}