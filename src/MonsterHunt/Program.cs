using System;
using System.Collections.Immutable;
using System.Linq;

namespace MonsterHunt
{
    class Program
    {
        static void Main(string[] args)
        {
            var town1 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Town 1"
            };

            var town2 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Town 2"
            };

            var route1 = new Route
            {
                Id = Guid.NewGuid(),
                Destination = town2
            };

            town1.Routes = ImmutableList.Create<Route>().Add(route1);

            var route2 = new Route
            {
                Id = Guid.NewGuid(),
                Destination = town1
            };

            var currentTown = town1;

            Console.WriteLine($"Welcome in {currentTown.Name}");

            var command = Console.ReadLine();

            if (command.StartsWith("go to "))
            {
                var destination = command.Substring(6);
                var route = currentTown.Routes.FirstOrDefault(t => t.Destination.Name.Equals(destination, StringComparison.InvariantCultureIgnoreCase));
                if (route == null)
                {
                    Console.WriteLine("Invalid town");
                }
                else
                {
                    currentTown = route.Destination;
                    Console.WriteLine($"Welcome in {currentTown.Name}");
                }
            }
        }
    }
}
