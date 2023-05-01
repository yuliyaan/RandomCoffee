using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomCoffee.Core.Abstractions;

namespace RandomCoffee.Tests.Fakes
{
    /// <summary>
    /// Fake implementation of IMessengerGateway for RandomCoffeeService testing
    /// </summary>
    public class FakeMessengerGateway : IMessengerGateway
    {
        private readonly List<string> _participants;
        
        public FakeMessengerGateway(string[] stubParticipants)
        {
            _participants = stubParticipants.ToList();
        }
        
        public async Task<string[]> GetRandomCoffeeParticipantIds()
        {
            return _participants.ToArray();
        }

        public async Task<bool> SendRandomCoffeeMessage(List<string[]> groups)
        {
            Console.WriteLine("");
            Console.WriteLine("Random coffee toss' results: ");
            
            groups.ForEach(x => Console.WriteLine(string.Join(" + ", x)));
            
            return true;
        }

        /// <summary>
        /// Add one or more participants to fake random coffee members list
        /// </summary>
        /// <param name="names">List of new members</param>
        public void AddParticipants(string[] names)
        {
            _participants.AddRange(names);
        }
        
        /// <summary>
        /// Remove one or more participants from fake random coffee members list
        /// </summary>
        /// <param name="names">List of members to remove</param>
        public void RemoveParticipants(string[] names)
        {
            foreach (var name in names)
            {
                _participants.Remove(name);
            }
        }
    }
}