using Payvision.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Service
{
    public class BitService : BaseService, IBitService
    {
        public BitService(ILoggerService loggerService) : base(loggerService)
        {
        }

        public IEnumerable<char> ConvertToBitArray(int input)
        {
            try
            {
                return Convert.ToString(input, 2).Select(i => i).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("Problem trying to convert the given Integer into a array of bits", ex);
                throw ex;
            }
        }

        public int GetBitOccurrences(IEnumerable<char> array, char bitToLookFor)
        {
            if (array == null || !array.Any())
                return 0;
                
            return array.Count(i => i == bitToLookFor);
        }

        public IEnumerable<int> GetBitPositions(IEnumerable<char> array, char bitToLookFor)
        {
            var result = new List<int>();

            if (array == null || !array.Any())
                return result;

            for (int i = 0; i < array.Count(); i++)
                if (array.ElementAt(i) == bitToLookFor)
                    result.Add(i);

            return result;
        }

        public IEnumerable<int> GetCountingBitsOutput(int bitOccurrences, IEnumerable<int> bitArray)
        {
            var result = new List<int> { bitOccurrences };

            if(bitArray != null && bitArray.Any())
                result.AddRange(bitArray);

            return result;
        }

        public IEnumerable<char> ReverseBitArray(IEnumerable<char> array)
        {
            if (array == null || !array.Any())
                return array;

            return array.Reverse();
        }

        public void ValidateInput(int input)
        {
            try
            {
                if(input < 0)
                    throw new ArgumentException("Invalid Input!");
            }
            catch(ArgumentException aex)
            {
                Logger.Error("User provided an invalid input", aex);
                throw aex;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
    }
}
