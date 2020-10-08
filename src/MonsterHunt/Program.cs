﻿using System;
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

            var merchant1 = new Merchant
            {
                Name = "Merchant 1",
                Requests = new List<ItemPrice>
                {
                    new ItemPrice
                    {
                        Item = item1,
                        Price = 1
                    },
                    new ItemPrice
                    {
                        Item = item3,
                        Price = 2
                    }
                }
            };

            var town1 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Town 1",
                Merchants = new List<Merchant> { merchant1 }
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
            Merchant currentMerchant = null;

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Attack = 1,
                Defense = 2,
                Health = 30,
                Inventory = new List<Item>()
            };

            Console.WriteLine($"Welcome in {currentTown.Name}");

            var dice = new Dice();

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
                    if (route != null)
                    {
                        currentTown = GoToTown(route, player, dice);
                        currentMerchant = null;
                        continue;
                    }

                    var merchant = currentTown.Merchants.FirstOrDefault(t => t.Name.Equals(destination, StringComparison.InvariantCultureIgnoreCase));
                    if (merchant != null)
                    {
                        currentMerchant = merchant;
                        Console.WriteLine($"Welcome to {merchant.Name}");
                        continue;
                    }
                    Console.WriteLine("Invalid town");
                }
                else if (command.Equals("inventory", StringComparison.InvariantCultureIgnoreCase))
                {
                    ShowInventory(player);
                }
                else if (command.Equals("requests", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (currentMerchant != null)
                    {
                        ShwoRequests(currentMerchant);
                    }
                    else
                    {
                        Console.WriteLine("Not at a merchant");
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

        private static Town GoToTown(Route route, Player player, Dice dice)
        {
            for (var i = 0; i < route.NumberOfMonsters; i++)
            {
                var monster = GetMonster(route, dice);

                Battle(player, monster, dice);
            }

            Console.WriteLine($"Welcome in {route.Destination.Name}");

            return route.Destination;
        }

        private static Monster GetMonster(Route route, Dice dice)
        {
            var monster = route.Monsters[dice.Roll() - 1];

            return new Monster
            {
                Name = monster.Name,
                Attack = monster.Attack,
                Defense = monster.Defense,
                Health = monster.Health,
                Loot = monster.Loot
            };
        }

        private static void Battle(Player player, Monster monster, Dice dice)
        {
            Console.WriteLine($"You encounter a {monster.Name}");
            foreach (var fightCommand in ReadLines())
            {
                if (fightCommand.Equals("attack", StringComparison.InvariantCultureIgnoreCase))
                {
                    var attackRollPlayer = dice.Roll();
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
                        var lootDice = dice.Roll();
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

                    var attackRollMonster = dice.Roll();
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

        private static void ShwoRequests(Merchant merchant)
        {
            foreach (var request in merchant.Requests)
            {
                Console.WriteLine($"{request.Item.Name} ({request.Price}c)");
            }
        }
    }
}
