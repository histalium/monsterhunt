using System;
using System.Collections.Generic;
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

            town2.Routes = ImmutableList.Create<Route>().Add(route2);

            var currentTown = town1;

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Attack = 1,
                Defense = 2,
                Health = 30
            };

            Console.WriteLine($"Welcome in {currentTown.Name}");

            var diceRolls = DiceRolls().GetEnumerator();
            diceRolls.MoveNext();

            foreach(var command in ReadLines())
            {
                if (command.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

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
                        var monster = new Monster
                        {
                            Name = "Monster 1",
                            Attack = 0,
                            Defense = 1,
                            Health = 15
                        };
                        Console.WriteLine($"You encounter a {monster.Name}");
                        foreach (var fightCommand in ReadLines())
                        {
                            if (fightCommand.Equals("attack", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var attackRollPlayer = diceRolls.Current;
                                diceRolls.MoveNext();
                                var attackPlayer = player.Attack + attackRollPlayer - monster.Defense;
                                if (attackPlayer > monster.Health)
                                {
                                    monster.Health = 0;
                                }
                                else
                                {
                                    monster.Health -= attackPlayer;
                                }
                                if (monster.Health == 0)
                                {
                                    Console.WriteLine($"{monster.Name} is defeated");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"{monster.Name}'s health is {monster.Health}");
                                }

                                var attackRollMonster = diceRolls.Current;
                                diceRolls.MoveNext();
                                var attackMonster = monster.Attack + attackRollMonster - player.Defense;
                                if (attackMonster > player.Health)
                                {
                                    player.Health = 0;
                                }
                                else
                                {
                                    player.Health -= attackMonster;
                                }
                                if (player.Health == 0)
                                {
                                    Console.WriteLine("You are defeated");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"Your health is {player.Health}");
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        currentTown = route.Destination;
                        Console.WriteLine($"Welcome in {currentTown.Name}");
                    }
                }
            }
        }

        private static IEnumerable<string> ReadLines()
        {
            while (true)
            {
                yield return Console.ReadLine();
            }
        }

        private static IEnumerable<int> DiceRolls()
        {
            var random = new Random();
            while (true)
            {
                yield return random.Next(6) + 1;
            }
        }
    }
}
