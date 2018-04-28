using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Interfaces
{
    public interface IBitService
    {
        /// <summary>
        /// Checks if the user provided a valid input, if not, throws the corresponding exception
        /// </summary>
        /// <param name="input"></param>
        void ValidateInput(int input);

        /// <summary>
        /// Converts the given int into a array of bits
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<char> ConvertToBitArray(int input);

        /// <summary>
        /// Reverses the provided array of bits
        /// </summary>
        /// <param name="array">Array to invert the order</param>
        /// <returns></returns>
        IEnumerable<char> ReverseBitArray(IEnumerable<char> array);

        /// <summary>
        /// Count the number of occurrences of the provided bit in the provided array
        /// </summary>
        /// <param name="array">Array to be used as a source</param>
        /// <param name="bitToLookFor">1 or 0</param>
        /// <returns></returns>
        int GetBitOccurrences(IEnumerable<char> array, char bitToLookFor);

        /// <summary>
        /// Looks for the given bit's position, in the provided array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="bitToLookFor"></param>
        /// <returns></returns>
        IEnumerable<int> GetBitPositions(IEnumerable<char> array, char bitToLookFor);

        /// <summary>
        /// Aggregate the number of bit occurrences, and the reversed bit array into one single array
        /// </summary>
        /// <param name="numberOfBitOcurrencies"></param>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        IEnumerable<int> GetCountingBitsOutput(int bitOccurrences, IEnumerable<int> bitArray);

    }
}
