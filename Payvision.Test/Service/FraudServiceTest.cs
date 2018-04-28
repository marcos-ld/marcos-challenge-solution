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
        private readonly IFraudService FraudService;

        public FraudServiceTest()
        {
            FraudService = new Payvision.Service.FraudService(new Payvision.Service.LoggerService());
        }

        [TestMethod]
        public void IsValidRequest_ShouldReturnFailedDirectoryValidation_IfInvalid()
        {
            var result = FraudService.IsValidRequest(new FraudRequest());

            Assert.IsFalse(result.Success, "The result is not the expected");
            Assert.AreEqual(result.Message, "Invalid Directory");
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void IsValidRequest_ShouldReturnFailedFileExtensionValidation_IfInvalid()
        {
            var result = FraudService.IsValidRequest(new FraudRequest
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
            var result = FraudService.IsValidRequest(new FraudRequest
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
            FraudService.EnsureFilePathIsValid(Path.Combine(Environment.CurrentDirectory, "Files", "OneLineFile123.txt"));
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void EnsureFilePathIsValid_ShouldTNothrowException_IfValid()
        {
            FraudService.EnsureFilePathIsValid(Path.Combine(Environment.CurrentDirectory, "Files", "OneLineFile.txt"));
        }

        [TestMethod]
        public void NormalizeEmailAddress_ShouldNormalizeEmailAsExpected()
        {
            var result = FraudService.NormalizeEmailAddress("marcos.vinicius.deus@gmail.com");

            Assert.AreEqual(result, "marcosviniciusdeus@gmail.com");
        }

        [TestMethod]
        public void NormalizeStateAddress_ShouldNormalizeStateAsExpected()
        {
            var result = FraudService.NormalizeStateAddress("il");

            Assert.AreEqual(result, "illinois");
        }

        [TestMethod]
        public void NormalizeStateAddress_ShouldNotNormalizeState_IfNotIncludedInKnownStates()
        {
            var result = FraudService.NormalizeStateAddress("tx");

            Assert.AreEqual(result, "tx");
        }

        [TestMethod]
        public void NormalizeStreetAddress_ShouldNormalizeStreetAsExcpected()
        {
            var result = FraudService.NormalizeStreetAddress("st. teding van jean");

            Assert.AreEqual(result, "street teding van jean");
        }

        [TestMethod]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void ReadOrders_ShouldReturnCorrespondingNUmberOfConvertedObjects()
        {
            var result = FraudService.ReadOrders(Path.Combine(Environment.CurrentDirectory, "Files", "FourLines_MoreThanOneFraudulent.txt"));

            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result[0].OrderId, 1);
            Assert.AreEqual(result[3].OrderId, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureFieldIsNumeric_ShouldThrownException_IfFieldInvalid()
        {
            FraudService.EnsureFieldIsNumeric("1asdasd313", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureFIeldIsFilled_MustThrowException_IfInvalid()
        {
            FraudService.EnsureFIeldIsFilled(null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureOrderHasAllMandatoryFields_ShouldThrowException_IfFieldNumberIsCorrect()
        {
            FraudService.EnsureOrderHasAllMandatoryFields(new string[] { "123", "marcosld@gmail.com", null, null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureOrderFieldsAreValid_ShouldThrowExceptionIfOneOfTheFieldsAreMissing()
        {
            FraudService.EnsureOrderFieldsAreValid("1233", "1", null, "street A", "city", "state 1");
        }

        [TestMethod]
        public void LookForCreditCardFraudByAddress_ShouldFail_IfThereAreFrauds()
        {
            //(order.DealId == orderToComp are.DealId
            //           && order.State == orderToCompare.State
            //           && order.ZipCode == orderToCompare.ZipCode
            //           && order.Street == orderToCompare.Street
            //           && order.City == orderToCompare.City
            //           && order.CreditCard != orderToCompare.CreditCard);

            var order1 = new Order()
            {
                DealId = 12,
                State = "TX",
                ZipCode = "2020",
                Street = "street 123",
                CreditCard = "2023213422445421"
            };

            var order2 = new Order()
            {
                DealId = 12,
                State = "TX",
                ZipCode = "2020",
                Street = "street 123",
                CreditCard = "2023213422445422"
            };

            var result = FraudService.LookForCreditCardFraudByAddress(order1, order2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByAddress_ShouldFSucceed_IfThereAreNoFrauds()
        {
            var order1 = new Order()
            {
                DealId = 13,
                State = "UT",
                ZipCode = "1232",
                Street = "street AA",
                CreditCard = "2443213422445421"
            };

            var order2 = new Order()
            {
                DealId = 13,
                State = "UT",
                ZipCode = "1232",
                Street = "street AA",
                CreditCard = "2443213422445421"
            };

            var result = FraudService.LookForCreditCardFraudByAddress(order1, order2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByEmail_ShouldFail_IfThereAreFrauds()
        {
            var order1 = new Order()
            {
                DealId = 28,
                Email = "marcosld@gmail.com",
                CreditCard = "2443213422445421"
            };

            var order2 = new Order()
            {
                DealId = 28,
                Email = "marcosld@gmail.com",
                CreditCard = "2443213422447721"
            };

            var result = FraudService.LookForCreditCardFraudByEmail(order1, order2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LookForCreditCardFraudByEmail_ShouldSucceed_IfThereAreNoFrauds()
        {
            var order1 = new Order()
            {
                DealId = 67,
                Email = "marcosld@gmail.com",
                CreditCard = "2443213422447721"
            };

            var order2 = new Order()
            {
                DealId = 67,
                Email = "marcosld@gmail.com",
                CreditCard = "2443213422447721"
            };

            var result = FraudService.LookForCreditCardFraudByEmail(order1, order2);

            Assert.IsFalse(result);
        }
    }
}
