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
    public class OrderServiceTest
    {
        private readonly IOrderService _orderService;

        public OrderServiceTest()
        {
            var loggerService = new Payvision.Service.LoggerService();

            _orderService = new Payvision.Service.OrderService(loggerService);
        }

        [TestMethod]
        public void NormalizeEmailAddress_ShouldNormalizeEmailAsExpected()
        {
            var result = _orderService.NormalizeEmailAddress("marcos.vinicius.deus@gmail.com");

            Assert.AreEqual(result, "marcosviniciusdeus@gmail.com");
        }

        [TestMethod]
        public void NormalizeStateAddress_ShouldNormalizeStateAsExpected()
        {
            var result = _orderService.NormalizeStateAddress("il");

            Assert.AreEqual(result, "illinois");
        }

        [TestMethod]
        public void NormalizeStateAddress_ShouldNotNormalizeState_IfNotIncludedInKnownStates()
        {
            var result = _orderService.NormalizeStateAddress("tx");

            Assert.AreEqual(result, "tx");
        }

        [TestMethod]
        public void NormalizeStreetAddress_ShouldNormalizeStreetAsExcpected()
        {
            var result = _orderService.NormalizeStreetAddress("st. teding van jean");

            Assert.AreEqual(result, "street teding van jean");
        }

        [TestMethod]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void ReadOrders_ShouldReturnCorrespondingNUmberOfConvertedObjects()
        {
            var result = _orderService.ReadOrders(Path.Combine(Environment.CurrentDirectory, "Files", "FourLines_MoreThanOneFraudulent.txt"));

            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result[0].OrderId, 1);
            Assert.AreEqual(result[3].OrderId, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureFieldIsNumeric_ShouldThrownException_IfFieldInvalid()
        {
            _orderService.EnsureFieldIsNumeric("1asdasd313", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureFIeldIsFilled_MustThrowException_IfInvalid()
        {
            _orderService.EnsureFIeldIsFilled(null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureOrderHasAllMandatoryFields_ShouldThrowException_IfFieldNumberIsCorrect()
        {
            _orderService.EnsureOrderHasAllMandatoryFields(new string[] { "123", "marcosld@gmail.com", null, null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureOrderFieldsAreValid_ShouldThrowExceptionIfOneOfTheFieldsAreMissing()
        {
            _orderService.EnsureOrderFieldsAreValid("1233", "1", null, "street A", "city", "state 1");
        }
    }
}
