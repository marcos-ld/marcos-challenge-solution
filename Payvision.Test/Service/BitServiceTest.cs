using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payvision.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Test.Service
{
    [TestClass]
    public class BitServiceTest
    {
        private readonly IBitService BitService;

        public BitServiceTest()
        {
            BitService = new Payvision.Service.BitService(new Payvision.Service.LoggerService());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateInput_Must_ThrowException_WithNegativeValues()
        {
            BitService.ValidateInput(-20);
        }

        [TestMethod]
        public void ConvertToBitArray_Should_CreateTheRightArray()
        {
            CollectionAssert.AreEqual(
                expected: new List<char> { '1', '1', '0', '0' },
                actual: BitService.ConvertToBitArray(12).ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void ReserveArray_MustReverseTheArray_AsExpected()
        {
            CollectionAssert.AreEqual(
                expected: new List<char> { '0', '0', '1', '1', '0', '0', '0', '0' },
                actual: BitService.ReverseBitArray(new List<char> { '0', '0', '0', '0', '1', '1', '0', '0' }).ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void ConvertToBitArray_Must_CountTheRightNumberOfBitOccorrences()
        {
            Assert.AreEqual(2, 
                BitService.GetBitOccurrences(new List<char> { '0', '0', '0', '0', '1', '1', '0', '0' }, '1'), 
                "The result is not the expected");
        }

        [TestMethod]
        public void GetBitPositions_Must_ReturnTheRightBitPositions()
        {
            CollectionAssert.AreEqual(
                expected: new List<int> { 4, 5 },
                actual: BitService.GetBitPositions(new List<char> { '0', '0', '0', '0', '1', '1', '0', '0' }, '1').ToList(),
                message: "The result is not the expected");
        }

        [TestMethod]
        public void GetCountingBitsOutput_Must_ReturnTheRightResult()
        {
            CollectionAssert.AreEqual(
                expected: new List<int> { 2, 4, 5 },
                actual: BitService.GetCountingBitsOutput(2, new List<int> { 4, 5 }).ToList(),
                message: "The result is not the expected");
        }
    }
}
