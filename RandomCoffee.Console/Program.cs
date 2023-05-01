using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RandomCoffee.Core.Services;
using RandomCoffee.EntityFramework;
using RandomCoffee.EntityFramework.Services;
using RandomCoffee.Mattermost;

namespace RandomCoffee.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            await using var context = new Db(configuration);
            var historian = new RandomCoffeeHistorian(context);
            var messengerGateway = new MessengerGateway(configuration);

            var randomCoffeeService = new RandomCoffeeService(historian, messengerGateway);

            await randomCoffeeService.Run();
        }
    }
}