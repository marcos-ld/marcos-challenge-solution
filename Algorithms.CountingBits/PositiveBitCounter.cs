// -----------------------------------------------------------------------
// <copyright file="BitCounter.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Algorithms.CountingBits
{
    using Payvision.Domain.Interfaces;
    using System;
    using System.Collections.Generic;
    using Autofac;

    public class PositiveBitCounter
    {
        private readonly IBitService _bitService;
        private readonly ILoggerService _loggerService;

        public PositiveBitCounter()
        {
            _bitService = Common.Ioc.ApplicationContainer.Resolve<IBitService>();
            _loggerService = Common.Ioc.ApplicationContainer.Resolve<ILoggerService>();
        }

        public PositiveBitCounter(IBitService bitService, ILoggerService loggerService)
        {
            _bitService = bitService;
            _loggerService = loggerService;
        }

        public IEnumerable<int> Count(int input)
        {
            try
            {
                _bitService.ValidateInput(input);

                var arrayOfBits = _bitService.ConvertToBitArray(input);

                // IN CASE OF NEED TO GET THE OCCURRENCES OF '0' it's possible
                var bitOccurrences = _bitService.GetBitOccurrences(arrayOfBits, '1');

                var reversedArrayOfBits = _bitService.ReverseBitArray(arrayOfBits);

                // IN CASE OF NEED TO GET THE POSITIONS OF '0' it's possible
                var bitPositions = _bitService.GetBitPositions(reversedArrayOfBits, '1');

                return _bitService.GetCountingBitsOutput(bitOccurrences, bitPositions);
            }
            catch(Exception ex)
            {
                _loggerService.Error("Problem on Counting Bits for '${input}'", ex);
                throw ex;
            }
        }
    }
}