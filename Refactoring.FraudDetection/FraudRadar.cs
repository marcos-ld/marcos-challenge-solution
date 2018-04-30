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
        private readonly IOrderService _orderService;
        private readonly ILoggerService _loggerService;

        public FraudRadar()
        {
            _fraudService = Common.Ioc.ApplicationContainer.Resolve<IFraudService>();
            _loggerService = Common.Ioc.ApplicationContainer.Resolve<ILoggerService>();
            _orderService = Common.Ioc.ApplicationContainer.Resolve<IOrderService>();
        }

        public FraudRadar(IFraudService fraudService, IOrderService orderService, ILoggerService loggerService)
        {
            _fraudService = fraudService;
            _loggerService = loggerService;
            _orderService = orderService;
        }

        public IEnumerable<FraudResult> Check(FraudRequest request)
        {
            try
            {
                // CHECK IF FILE EXISTS AND IF THE EXTESION IS VALID
                request.Validate(_fraudService);

                var result = new List<FraudResult>();

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
                var orders = _orderService.ReadOrders(filePath);

                // NORMALIZE
                foreach (var order in orders)
                    order.Normalize(_orderService);

                // CHECK FRAUD
                return RunAnalysis(orders, filePath);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Problem on Checking for Frauds on file '${filePath}'", ex);
                throw ex;
            }
        }

        public IList<FraudResult> RunAnalysis(IList<Order> orders, string file)
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
                        fraudResults.Add(new FraudResult(file, orderToCompare.OrderId, true));
                }
            }

            return fraudResults;
        }
    }
}