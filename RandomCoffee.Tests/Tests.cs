using RandomCoffee.Core.Services;
using Xunit;
using RandomCoffee.Tests.Fakes;

namespace RandomCoffee.Tests
{
    public class Tests
    {
        /// <summary>
        /// The test runs Random Coffee n times (specified in constant below).
        /// At every toss Random Coffee participants' count may change.
        /// Results of Random Coffee toss are sent to console.
        /// </summary>
        [Fact]
        public async Task BigTest()
        {
            const int participantsCountAtStart = 10;
            const int randomCoffeeTossCount = 20;
            
            var rnd = new Random();
            var messenger = new FakeMessengerGateway(_names.OrderBy(_ => rnd.Next()).Take(participantsCountAtStart).ToArray());
            var historian = new FakeHistorian();
            var randomCoffeeService = new RandomCoffeeService(historian, messenger);

            for (int i = 0; i < randomCoffeeTossCount; i++)
            {
                await DoChangeInCount(rnd, messenger);
                
                // Act:
                var result = randomCoffeeService.Run();
            }

            var participants = await messenger.GetRandomCoffeeParticipantIds();
        }

        /// <summary>
        /// Randomly add or remove random coffee members
        /// </summary>
        private async Task DoChangeInCount(Random rnd, FakeMessengerGateway messenger)
        {
            var changeInCount = rnd.Next(-2, 2);
            if (changeInCount == 0)
            {
                return;
            }

            var currentParticipants = await messenger.GetRandomCoffeeParticipantIds();

            if (changeInCount > 0)
            {
                var possibleNewParticipants = _names.Except(currentParticipants).ToList();
                var newParticipants = possibleNewParticipants.OrderBy(_ => rnd.Next()).Take(changeInCount).ToArray();

                messenger.AddParticipants(newParticipants);
            }
            else
            {
                var participantsToExclude = currentParticipants.OrderBy(_ => rnd.Next()).Take(changeInCount).ToArray();

                messenger.RemoveParticipants(participantsToExclude);
            }
        }

        /// <summary>
        /// List of names for Random Coffee
        /// </summary>
        private readonly List<string> _names = new()
        {
            "Jacob","Michael","Joshua","Matthew","Ethan","Andrew","Daniel","William","Joseph","Christopher",
            "Emily","Emma","Madison","Olivia","Hannah","Abigail","Isabella","Ashley","Samantha","Elizabeth",
            "Anthony","Ryan","Nicholas","David","Alexander","Tyler","James","John","Dylan","Nathan",
            "Elizabeth","Alexis","Sarah","Grace","Alyssa","Sophia","Lauren","Brianna","Kayla","Natalie",
            "Nathan","Jonathan","Brandon","Samuel","Christian","Benjamin","Zachary","Logan","Jose","Noah","Justin",
            "Anna","Jessica","Taylor","Chloe","Hailey","Ava","Jasmine","Sydney","Victoria","Ella","Mia"
        };
    }
}
