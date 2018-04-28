// -----------------------------------------------------------------------
// <copyright file="PositiveBitCounterTest.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace CountingBits.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Payvision.CodeChallenge.Algorithms.CountingBits;
    using Payvision.Domain.Interfaces;

    [TestClass]
    public class PositiveBitCounterTest
    {
        private readonly PositiveBitCounter bitCounter = new PositiveBitCounter();

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Count_NegativeValue_ArgumentExceptionExpected()
        {
            this.bitCounter.Count(-2);
        }

        [TestMethod]
        public void Count_Zero_NoOccurrences()
        {
            CollectionAssert.AreEqual(
                expected: new List<int> { 0 },
                actual: this.bitCounter.Count(0).ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void Count_ValidInput_OneOcurrence()
        {
            CollectionAssert.AreEqual(
                expected: new List<int> { 1, 0 },
                actual: this.bitCounter.Count(1).ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void Count_ValidInput_MultipleOcurrence()
        {
            CollectionAssert.AreEqual(
                expected: new List<int> { 3, 0, 5, 7 },
                actual: this.bitCounter.Count(161).ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void Count_ValidInput_MandatoryMethodsMustHaveBeenUsed()
        {
            var bitService = A.Fake<IBitService>();

            var bitCounter = new PositiveBitCounter(bitService, A.Fake<ILoggerService>());

            bitCounter.Count(161);

            A.CallTo(() => bitService.ValidateInput(A<int>.That.IsEqualTo(161))).MustHaveHappenedOnceExactly();
            A.CallTo(() => bitService.GetBitOccurrences(A<IEnumerable<char>>._, A<char>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bitService.ReverseBitArray(A<IEnumerable<char>>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bitService.GetBitPositions(A<IEnumerable<char>>._, A<char>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bitService.GetCountingBitsOutput(A<int>._, A<IEnumerable<int>>._)).MustHaveHappenedOnceExactly();
        }
    }
}