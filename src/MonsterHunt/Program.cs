using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MonsterHunt
{
    class Program
    {
        private static List<Item> items;
        private static List<Monster> monsters;

        static void Main(string[] args)
        {
            var item1 = CreateItem("Item 1");
            var item2 = CreateItem("Item 2");
            var item3 = CreateItem("Item 3");
            var item4 = CreateItem("Item 4");

            items = new List<Item> { item1, item2, item3, item4 };

            var monster1 = new Monster
            {
                Id = Guid.NewGuid(),
                Name = "Monster 1",
                Attack = 0,
                Defense = 1,
                Health = 15,
                Loot = new RollResult()
                    .Set(1, item1.Id)
                    .Set(2, item2.Id)
            };

            var monster2 = new Monster
            {
                Id = Guid.NewGuid(),
                Name = "Monster 2",
                Attack = 1,
                Defense = 1,
                Health = 12,
                Loot = new RollResult()
                    .Set(1, item1.Id)
                    .Set(2, item3.Id)
            };

            monsters = new List<Monster> { monster1, monster2 };

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
                },
                Offers = new List<ItemPrice>
                {
                    new ItemPrice
                    {
                        Item = item4,
                        Price = 5
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
                Monsters = new RollResult()
                    .Set(1, 4, monster1.Id)
                    .Set(5, 6, monster2.Id)
            };

            town1.Routes = ImmutableList.Create<Route>().Add(route1);

            var route2 = new Route
            {
                Id = Guid.NewGuid(),
                Destination = town1,
                NumberOfMonsters = 2,
                Monsters = new RollResult()
                    .Set(1, 4, monster1.Id)
                    .Set(5, 6, monster2.Id)
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
                Inventory = new List<Item>(),
                Coins = 10
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
                else if (command.Equals("offers", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (currentMerchant != null)
                    {
                        ShwoOffers(currentMerchant);
                    }
                    else
                    {
                        Console.WriteLine("Not at a merchant");
                    }
                }
                else if (command.StartsWith("sell ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var itemName = command.Substring(5);
                    var item = items.Where(t => t.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (item == null)
                    {
                        Console.WriteLine("Invalid item");
                    }
                    else if (currentMerchant == null)
                    {
                        Console.WriteLine("Not at a merchant");
                    }
                    else if (!player.Inventory.Contains(item))
                    {
                        Console.WriteLine("You does not have item");
                    }
                    else if (!currentMerchant.Requests.Where(t => t.Item == item).Any())
                    {
                        Console.WriteLine("Merchant does not request item");
                    }
                    else
                    {
                        var value = currentMerchant.Requests.Where(t => t.Item == item).Single().Price;
                        player.Inventory.Remove(item);
                        player.Coins += value;
                        Console.WriteLine($"Item sold. You have now {player.Coins}c");
                    }
                }
                else if (command.StartsWith("buy ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var itemName = command.Substring(4);
                    var item = items.Where(t => t.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (item == null)
                    {
                        Console.WriteLine("Invalid item");
                    }
                    else if (currentMerchant == null)
                    {
                        Console.WriteLine("Not at a merchant");
                    }
                    else if (!currentMerchant.Offers.Where(t => t.Item == item).Any())
                    {
                        Console.WriteLine("Merchant does not offer item");
                    }
                    else if (player.Coins < currentMerchant.Offers.Where(t => t.Item == item).Single().Price)
                    {
                        Console.WriteLine("You don't have enough coins");
                    }
                    else
                    {
                        var value = currentMerchant.Offers.Where(t => t.Item == item).Single().Price;
                        player.Inventory.Add(item);
                        player.Coins -= value;
                        Console.WriteLine($"Bought item. You have now {player.Coins}c");
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

        private static Item CreateItem(string name)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            return item;
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
            var diceRoll = dice.Roll();
            var monsterId = route.Monsters.GetResult(diceRoll);

            if (!monsterId.HasValue)
            {
                return null;
            }

            var monster = monsters.Where(t => t.Id == monsterId).Single();

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
                        var loot = GetLoot(monster, dice);
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

        private static Item GetLoot(Monster monster, Dice dice)
        {
            var lootDice = dice.Roll();
            var lootId = monster.Loot.GetResult(lootDice);

            if (!lootId.HasValue)
            {
                return null;
            }

            var loot = items.Where(t => t.Id == lootId).Single();
            return loot;
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

        private static void ShwoOffers(Merchant merchant)
        {
            foreach (var request in merchant.Offers)
            {
                Console.WriteLine($"{request.Item.Name} ({request.Price}c)");
            }
        }
    }
}
