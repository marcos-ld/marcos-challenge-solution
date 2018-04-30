using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payvision.Domain.Interfaces;
using Payvision.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Payvision.Domain.Entities;

namespace Payvision.Test.Service
{
    [TestClass]
    public class FraudServiceTest
    {
        private readonly IFraudService _fraudService;

        public FraudServiceTest()
        {
            _fraudService = new Payvision.Service.FraudService(new Payvision.Service.LoggerService());
        }

        [TestMethod]
        public void IsValidRequest_ShouldReturnFailedDirectoryValidation_IfInvalid()
        {
            var result = _fraudService.IsValidRequest(new FraudRequest());

            Assert.IsFalse(result.Success, "The result is not the expected");
            Assert.AreEqual(result.Message, "Invalid Directory");
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void IsValidRequest_ShouldReturnFailedFileExtensionValidation_IfInvalid()
        {
            var result = _fraudService.IsValidRequest(new FraudRequest
            {
                Directory = Path.Combine(Environment.CurrentDirectory, "Files")
            });

            Assert.IsFalse(result.Success, "The result is not the expected");
            Assert.AreEqual(result.Message, "Invalid Search Pattern Extension, and/or File. *.txt Allowed only");
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void IsValidRequest_ShouldSucceedValidation_IfCorrect()
        {
            var result = _fraudService.IsValidRequest(new FraudRequest
            {
                Directory = Path.Combine(Environment.CurrentDirectory, "Files"),
                FileName = "OneLineFile.txt"
            });

            Assert.IsTrue(result.Success, "The result is not the expected");
            Assert.IsNull(result.Message);
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureFilePathIsValid_ShouldThrowException_IfInvalid()
        {
            _fraudService.EnsureFilePathIsValid(Path.Combine(Environment.CurrentDirectory, "Files", "OneLineFile123.txt"));
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void EnsureFilePathIsValid_ShouldTNothrowException_IfValid()
        {
            _fraudService.EnsureFilePathIsValid(Path.Combine(Environment.CurrentDirectory, "Files", "OneLineFile.txt"));
        }

        [TestMethod]
        public void LookForCreditCardFraudByAddress_ShouldFail_IfThereAreFrauds()
        {
            var order1 = new Order(
                orderId: 1,
                dealId: 12,
                email: string.Empty,
                city: "city1",
                state: "TX",
                zipCode: "2020",
                street: "street 123",
                creditCard: "2023213422445421"
            );

            var order2 = new Order(
                orderId: 2,
                dealId: 12,
                email: string.Empty,
                city: "city1",
                state: "TX",
                zipCode: "2020",
                street: "street 123",
                creditCard: "2023213422445422"
            );

            var result = _fraudService.LookForCreditCardFraudByAddress(order1, order2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByAddress_ShouldFSucceed_IfThereAreNoFrauds()
        {
            var order1 = new Order(
                orderId: 1,
                dealId: 13,
                email: string.Empty,
                city: "ab",
                street: "street AA",
                state: "UT",
                zipCode: "1232",
                creditCard: "2443213422445421"
            );

            var order2 = new Order(
                orderId: 2,
                dealId: 13,
                email: string.Empty,
                city: "ab",
                state: "UT",
                zipCode: "1232",
                street: "street AA",
                creditCard: "2443213422445421"
            );

            var result = _fraudService.LookForCreditCardFraudByAddress(order1, order2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByEmail_ShouldFail_IfThereAreFrauds()
        {
            var order1 = new Order(
                orderId: 2,
                dealId: 28,
                email: "marcosld@gmail.com",
                street: null,
                city: null,
                state: null,
                zipCode: null,
                creditCard: "2443213422445421"
            );

            var order2 = new Order(
                orderId: 3,
                dealId: 28,
                email: "marcosld@gmail.com",
                street: null,
                city: null,
                state: null,
                zipCode: null,
                creditCard: "2443213422447721"
            );

            var result = _fraudService.LookForCreditCardFraudByEmail(order1, order2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByEmail_ShouldSucceed_IfThereAreNoFrauds()
        {
            var order1 = new Order(
                orderId: 1,
                dealId: 67,
                email: "marcosld@gmail.com",
                street: null,
                city: null,
                state: null,
                zipCode: null,
                creditCard: "2443213422447721"
            );

            var order2 = new Order(
                orderId: 2,
                dealId: 67,
                email: "marcosld@gmail.com",
                street: null,
                city: null,
                state: null,
                zipCode: null,
                creditCard: "2443213422447721"
            );

            var result = _fraudService.LookForCreditCardFraudByEmail(order1, order2);

            Assert.IsFalse(result);
        }
    }
}
