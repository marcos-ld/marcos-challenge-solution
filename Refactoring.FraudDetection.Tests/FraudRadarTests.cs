﻿// -----------------------------------------------------------------------
// <copyright file="FraudRadarTests.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Refactoring.FraudDetection.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Payvision.Domain.Entities;
    using Payvision.Domain.Interfaces;
    using Payvision.Domain.Model;

    [TestClass]
    public class FraudRadarTests
    {
        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void CheckFraud_OneLineFile_NoFraudExpected()
        {
            var result = ExecuteTest("OneLineFile.txt");

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(0, "The result should not contains fraudulent lines");
        }

        [TestMethod]
        [DeploymentItem("./Files/TwoLines_FraudulentSecond.txt", "Files")]
        public void CheckFraud_TwoLines_SecondLineFraudulent()
        {
            var result = ExecuteTest("TwoLines_FraudulentSecond.txt");

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem("./Files/ThreeLines_FraudulentSecond.txt", "Files")]
        public void CheckFraud_ThreeLines_SecondLineFraudulent()
        {
            var result = ExecuteTest("ThreeLines_FraudulentSecond.txt");

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void CheckFraud_FourLines_MoreThanOneFraudulent()
        {
            var result = ExecuteTest("FourLines_MoreThanOneFraudulent.txt");

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(2, "The result should contains the number of lines of the file");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFraud_InvalidFileExtension_ShouldThrowException()
        {
            var result = ExecuteTest("FourLines_MoreThanOneFraudulent.csv");

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(2, "The result should contains the number of lines of the file");
        }

        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        [DeploymentItem("./Files/TwoLines_FraudulentSecond.txt", "Files")]
        [DeploymentItem("./Files/ThreeLines_FraudulentSecond.txt", "Files")]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void CheckFraud_MustHaveReadOrdersFourtimes()
        {
            var fraudService = A.Fake<IFraudService>();
            var orderService = A.Fake<IOrderService>();

            //Arrange
            A.CallTo(() => fraudService.IsValidRequest(A<FraudRequest>._)).Returns(new FraudRequestValidation(true));
            A.CallTo(() => orderService.ReadOrders(A<string>._)).Returns(new List<Order>
            {
                new Order ( orderId: 1, dealId: 1, email: "bugs@bunny.com", street: "123 Sesame St.", city: "New York", state: "NY", zipCode: "10011", creditCard: "12345689010" ),
                new Order ( orderId: 2, dealId: 1, email: "bugs@bunny.com", street: "123 Sesame St.", city: "New York", state: "NY", zipCode: "10011", creditCard: "12345689011" ),
                new Order ( orderId: 3, dealId: 2, email: "roger@rabbit.com", street: "1234 Not Sesame St.", city: "Colorado", state: "NY", zipCode: "10012", creditCard: "12345689012" ),
                new Order ( orderId: 4, dealId: 2, email : "roger@rabbit.com", street : "1234 Not Sesame St.", city : "Colorado", state : "NY", zipCode : "10012", creditCard : "12345689014" )
            });

            var fraudRadar = new FraudRadar(fraudService, orderService, A.Fake<ILoggerService>());

            //Act
            fraudRadar.Check(new FraudRequest
            {
                Directory = Path.Combine(Environment.CurrentDirectory, "Files"),
                SearchPattern = "*.txt"
            }).ToList();

            A.CallTo(() => fraudService.IsValidRequest(A<FraudRequest>._)).MustHaveHappenedOnceExactly();

            A.CallTo(() => fraudService.EnsureFilePathIsValid(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(4));
            A.CallTo(() => orderService.ReadOrders(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(4));

            A.CallTo(() => orderService.NormalizeEmailAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(16));
            A.CallTo(() => orderService.NormalizeStateAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(16));
            A.CallTo(() => orderService.NormalizeStreetAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(16));

            A.CallTo(() => fraudService.LookForCreditCardFraudByEmail(A<Order>._, A<Order>._)).MustHaveHappened(Repeated.AtLeast.Times(4));
            A.CallTo(() => fraudService.LookForCreditCardFraudByAddress(A<Order>._, A<Order>._)).MustHaveHappened(Repeated.AtLeast.Times(4));
        }

        [TestMethod]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void Count_ValidInput_MandatoryMethodsMustHaveBeenUsed()
        {
            var fraudService = A.Fake<IFraudService>();
            var orderService = A.Fake<IOrderService>();

            //Arrange
            A.CallTo(() => fraudService.IsValidRequest(A<FraudRequest>._)).Returns(new FraudRequestValidation(true));
            A.CallTo(() => orderService.ReadOrders(A<string>._)).Returns(new List<Order>
            {
                new Order ( orderId: 1, dealId: 1, email: "bugs@bunny.com", street: "123 Sesame St.", city: "New York", state: "NY", zipCode: "10011", creditCard: "12345689010" ),
                new Order ( orderId: 2, dealId: 1, email: "bugs@bunny.com", street: "123 Sesame St.", city: "New York", state: "NY", zipCode: "10011", creditCard: "12345689011" ),
                new Order ( orderId: 3, dealId: 2, email: "roger@rabbit.com", street: "1234 Not Sesame St.", city: "Colorado", state: "NY", zipCode: "10012", creditCard: "12345689012" ),
                new Order ( orderId: 4, dealId: 2, email: "roger@rabbit.com", street: "1234 Not Sesame St.", city: "Colorado", state: "NY", zipCode: "10012", creditCard: "12345689014" )
            });

            var fraudRadar = new FraudRadar(fraudService, orderService, A.Fake<ILoggerService>());

            //Act
            fraudRadar.Check(new FraudRequest
            {
                Directory = Path.Combine(Environment.CurrentDirectory, "Files"),
                FileName = "FourLines_MoreThanOneFraudulent.txt"
            }).ToList();

            //Assert
            A.CallTo(() => fraudService.IsValidRequest(A<FraudRequest>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => orderService.ReadOrders(A<string>._)).MustHaveHappenedOnceExactly();

            A.CallTo(() => orderService.NormalizeEmailAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(4));
            A.CallTo(() => orderService.NormalizeStateAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(4));
            A.CallTo(() => orderService.NormalizeStreetAddress(A<string>._)).MustHaveHappened(Repeated.Exactly.Times(4));

            A.CallTo(() => fraudService.LookForCreditCardFraudByEmail(A<Order>._, A<Order>._)).MustHaveHappened(Repeated.AtLeast.Once);
            A.CallTo(() => fraudService.LookForCreditCardFraudByAddress(A<Order>._, A<Order>._)).MustHaveHappened(Repeated.AtLeast.Once);
        }

        private static List<FraudResult> ExecuteTest(string file)
        {
            var fraudRadar = new FraudRadar();

            // YOU CAN EITHER SPECIFY THE FILE NAME, OR THE FILE EXTENSION ONLY
            // I.E. SearchPattern = "*.txt", or SearchPattern = "FourLines_MoreThanOneFraudulent.txt"
            return fraudRadar.Check(new FraudRequest
            {
                Directory = Path.Combine(Environment.CurrentDirectory, "Files"),
                FileName = file
            }).ToList();
        }
    }
}