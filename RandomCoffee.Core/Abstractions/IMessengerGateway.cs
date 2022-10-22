using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomCoffee.Core.Abstractions
{
    /// <summary>
    /// Connector for Random Coffee to a specific messenger 
    /// </summary>
    public interface IMessengerGateway
    {
        /// <summary>
        /// Get list of Random Coffee participants
        /// </summary>
        /// <returns>The collection of Random Coffee's participants ids</returns>
        Task<string[]> GetRandomCoffeeParticipantIds();

        /// <summary>
        /// Send message with Random Coffee's toss info
        /// </summary>
        /// <param name="groups">Random Coffee participants groups info. Each group contains 2 or 3 participants.</param>
        /// <returns>true if message successfully sent</returns>
        Task<bool> SendRandomCoffeeMessage(List<string[]> groups);
    }
}