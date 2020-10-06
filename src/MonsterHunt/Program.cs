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
            var item1 = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Item 1"
            };

            var item2 = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Item 2"
            };

            var item3 = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Item 3"
            };

            var monster1 = new Monster
            {
                Name = "Monster 1",
                Attack = 0,
                Defense = 1,
                Health = 15,
                Loot = new[] {
                    item1, item2, null, null, null, null
                }
            };

            var monster2 = new Monster
            {
                Name = "Monster 2",
                Attack = 1,
                Defense = 1,
                Health = 12,
                Loot = new[] {
                    item1, item3, null, null, null, null
                }
            };

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
                Destination = town2,
                NumberOfMonsters = 2,
                Monsters = new[] {
                    monster1,
                    monster1,
                    monster1,
                    monster1,
                    monster2,
                    monster2
                }
            };

            town1.Routes = ImmutableList.Create<Route>().Add(route1);

            var route2 = new Route
            {
                Id = Guid.NewGuid(),
                Destination = town1,
                NumberOfMonsters = 2,
                Monsters = new[] {
                    monster1,
                    monster1,
                    monster1,
                    monster1,
                    monster2,
                    monster2
                }
            };

            town2.Routes = ImmutableList.Create<Route>().Add(route2);

            var currentTown = town1;

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Attack = 1,
                Defense = 2,
                Health = 30,
                Inventory = new List<Item>()
            };

            Console.WriteLine($"Welcome in {currentTown.Name}");

            var diceRolls = DiceRolls().GetEnumerator();
            diceRolls.MoveNext();

            foreach (var command in ReadLines())
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
                        currentTown = GoToTown(route, player, diceRolls);
                    }
                }
                else if (command.Equals("inventory", StringComparison.InvariantCultureIgnoreCase))
                {
                    ShowInventory(player);
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

        private static Town GoToTown(Route route, Player player, IEnumerator<int> diceRolls)
        {
            for (var i = 0; i < route.NumberOfMonsters; i++)
            {
                var monster = GetMonster(route, diceRolls);

                Battle(player, monster, diceRolls);
            }

            Console.WriteLine($"Welcome in {route.Destination.Name}");

            return route.Destination;
        }

        private static Monster GetMonster(Route route, IEnumerator<int> diceRolls)
        {
            var monster = route.Monsters[diceRolls.Current - 1];
            diceRolls.MoveNext();

            return new Monster
            {
                Name = monster.Name,
                Attack = monster.Attack,
                Defense = monster.Defense,
                Health = monster.Health,
                Loot = monster.Loot
            };
        }

        private static void Battle(Player player, Monster monster, IEnumerator<int> diceRolls)
        {
            Console.WriteLine($"You encounter a {monster.Name}");
            foreach (var fightCommand in ReadLines())
            {
                if (fightCommand.Equals("attack", StringComparison.InvariantCultureIgnoreCase))
                {
                    var attackRollPlayer = diceRolls.Current;
                    diceRolls.MoveNext();
                    var attackPlayer = player.Attack + attackRollPlayer - monster.Defense;
                    attackPlayer = Math.Max(0, attackPlayer);
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
                        var lootDice = diceRolls.Current;
                        diceRolls.MoveNext();
                        var loot = monster.Loot[lootDice - 1];
                        if (loot != null)
                        {
                            Console.WriteLine($"loot: {loot.Name}");
                            player.Inventory.Add(loot);
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{monster.Name}'s health is {monster.Health}");
                    }

                    var attackRollMonster = diceRolls.Current;
                    diceRolls.MoveNext();
                    var attackMonster = monster.Attack + attackRollMonster - player.Defense;
                    attackMonster = Math.Max(0, attackMonster);
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
        }

        private static void ShowInventory(Player player)
        {
            Console.WriteLine("Inventory");

            foreach (var item in player.Inventory)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}
