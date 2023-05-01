using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomCoffee.Core.Abstractions;

namespace RandomCoffee.Core.Services
{
    /// <summary>
    /// Main Random Coffee service
    /// </summary>
    public class RandomCoffeeService
    {
        private readonly IRandomCoffeeHistorian _historian;
        private readonly IMessengerGateway _messenger;
        private readonly Random _random;

        public RandomCoffeeService(IRandomCoffeeHistorian historian, IMessengerGateway messenger)
        {
            _historian = historian;
            _messenger = messenger;
            _random = new Random();
        }

        /// <summary>
        /// Run random coffee
        /// </summary>
        public async Task Run()
        {
            var randomCoffeeMembers = await _messenger.GetRandomCoffeeParticipantIds();
            if (randomCoffeeMembers.Length < 2)
            {
                return; // No sense of toss if only 1 participant
            }

            var coffeeHistory = GetRandomCoffeeHistory(randomCoffeeMembers.Length);
            var membersData = GetMembersData(randomCoffeeMembers, coffeeHistory);

            var result = Toss(membersData);

            await _messenger.SendRandomCoffeeMessage(result);
            // Save to historian only if message send was successful
            _historian.SaveRandomCoffeeInfo(result);
        }

        /// <summary>
        /// Start random coffee toss
        /// </summary>
        private List<string[]> Toss(List<MemberData> members)
        {
            var result = new List<string[]>();
            var count = Math.Floor(members.Count / 2.0);

            for (var i = 1; i <= count; i++)
            {
                if (members!.Count <= 3) // If only <=3 members left, just add them into 1 group
                {
                    result.Add(members.Select(x => x.Member).ToArray());
                    break;
                }
                
                // Sort members in order of increasing number of potential "partners" for random coffee,
                // because the less potential partners member has, the harder to find their a partner
                members = members.OrderBy(x => x.PossibleCompanions.Count).ThenBy(_ => _random.Next()).ToList();
                
                var currentMember = members.FirstOrDefault();
                if (!currentMember!.PossibleCompanions.Any())
                {
                    // If a member has no possible partners, with whom their haven't had coffee yet, just take any member as their partner
                    currentMember!.PossibleCompanions
                        .AddRange(members.Where(x => x.Member != currentMember.Member).Select(x => x.Member).ToList());
                }

                // Select as a partner a random member from PossibleCompanions
                var companion = currentMember.PossibleCompanions[_random.Next(currentMember.PossibleCompanions.Count)];
                result.Add(new [] { currentMember.Member, companion });
                
                // Remove current member and their partner from the list of random coffee members (because they already have a partner)
                members.Remove(currentMember);
                members.Remove(members.FirstOrDefault(x => x.Member == companion));
                
                // Remove current member and their partner from all the other members' possible partners' lists
                members.ForEach(d => d.PossibleCompanions.Remove(currentMember.Member));
                members.ForEach(d => d.PossibleCompanions.Remove(companion));
            }

            // Shuffle results to make them more random
            result = result.OrderBy(_ => _random.Next()).ToList();
            result.ForEach(x => x = x.OrderBy(_ => _random.Next()).ToArray());

            return result;
        }

        /// <summary>
        /// Get random coffee's members info with the lists of their possible partners
        /// </summary>
        private List<MemberData> GetMembersData(string[] randomCoffeeMembers, List<List<string[]>> coffeeHistory)
        {
            var members = new List<MemberData>();
            foreach (var member in randomCoffeeMembers)
            {
                var alreadyHadCoffeeWith = coffeeHistory
                    .SelectMany(x => x)
                    .Where(x => x.Any(m => m == member))
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();

                var possibleCompanions = randomCoffeeMembers
                    .Except(alreadyHadCoffeeWith)
                    .Except(Enumerable.Repeat(member, 1)) // Remove "myself" from possible partners list 
                    .ToList();
                
                var data = new MemberData
                {
                    Member = member,
                    PossibleCompanions = possibleCompanions
                };
                
                members.Add(data);
            }

            return members;
        }

        private List<List<string[]>> GetRandomCoffeeHistory(int membersCount)
        {
            var possibleCompanionsCount = membersCount - 1; 
            
            // Assume not to take into account records older then half a year (if random coffee's toss happens every week)
            // 52 weeks in a year divided by 2 = 26
            const int maxRecordsCount = 26;
            if (possibleCompanionsCount > maxRecordsCount) 
            {
                possibleCompanionsCount = maxRecordsCount;
            }
            
            // We need magic number "2" in the next line for the case if the list of coffee participants remains
            // the same on the 2nd, 3d etc. rounds. If we extract history with "1" (not "2") records removed,
            // coffee's 2nd round will exactly repeat 1st one. So we remove 2 records to make random coffee more "random"
            
            return _historian.GetRandomCoffeeHistory(possibleCompanionsCount - 2);
        }

        private class MemberData
        {
            /// <summary>
            /// Random coffee participant
            /// </summary>
            public string Member { get; set; }

            /// <summary>
            /// List of members, with whom current member haven't had coffee yet (according to coffee history)
            /// </summary>
            public List<string> PossibleCompanions { get; set; }
        }
    }
}