using System.Collections.Generic;

namespace RandomCoffee.Core.Abstractions
{
    /// <summary>
    /// Represents a storage of Random Coffee's info
    /// </summary>
    public interface IRandomCoffeeHistorian
    {
        /// <summary>
        /// Get info about previous tosses of Random Coffee
        /// </summary>
        /// <param name="count">Count of tosses to get info of (starting from the recent one)</param>
        /// <returns>The collection of previous tosses infos. Each toss info is represented by collection of string arrays.
        /// The arrays contain ids of Random Coffee participants, which were in one group of two or three in that toss.</returns>
        List<List<string[]>> GetRandomCoffeeHistory(int count);

        /// <summary>
        /// Save info about one Random Coffee's toss
        /// </summary>
        /// <param name="groups">This toss info. Expected the collection of string arrays, each array - one group of two or three participants in that toss.</param>
        void SaveRandomCoffeeInfo(List<string[]> groups);
    }
}