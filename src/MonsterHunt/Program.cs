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
        private static List<Town> towns;
        private static List<Route> routes;
        private static List<Merchant> merchants;

        static void Main(string[] args)
        {
            var item1 = CreateItem("Item 1");
            var item2 = CreateItem("Item 2");
            var item3 = CreateItem("Item 3");
            var item4 = CreateHealthPotion("Health potion 1", 5);

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

            towns = new List<Town> { town1, town2 };

            var route1 = new Route
            {
                Id = Guid.NewGuid(),
                Towns = new List<Guid>
                {
                    town1.Id, town2.Id
                },
                NumberOfMonsters = 2,
                Monsters = new RollResult()
                    .Set(1, 4, monster1.Id)
                    .Set(5, 6, monster2.Id)
            };

            routes = new List<Route> { route1 };

            var merchant1 = new Merchant
            {
                Name = "Merchant 1",
                TownId = town1.Id,
                Requests = new List<ItemPrice>
                {
                    new ItemPrice
                    {
                        ItemId = item1.Id,
                        Price = 1
                    },
                    new ItemPrice
                    {
                        ItemId = item3.Id,
                        Price = 2
                    }
                },
                Offers = new List<ItemPrice>
                {
                    new ItemPrice
                    {
                        ItemId = item4.Id,
                        Price = 5
                    }
                }
            };

            merchants = new List<Merchant> { merchant1 };

            var game = new MonsterHuntGame(towns, routes, monsters, items, merchants);
            game.MonsterDefeated += MonsterDefeated;

            Console.WriteLine($"Welcome in {game.CurrentTown.Name}");

            foreach (var command in ReadLines())
            {
                if (command.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                try
                {
                    if (command.StartsWith("go to "))
                    {
                        var destination = command.Substring(6);
                        try
                        {
                            game.GoToTown(destination);

                            if (game.CurrentMonster != null)
                            {
                                //todo event
                                Console.WriteLine($"You encounter a {game.CurrentMonster.Name}");
                            }

                            continue;
                        }
                        catch (InvalidTownException)
                        {
                            // do nothing. try other command.
                        }
                        catch (SameTownException)
                        {
                            Console.WriteLine("Same town as current");
                            continue;
                        }
                        catch (NoRouteToTownException)
                        {
                            Console.WriteLine("No route to town");
                        }

                        try
                        {
                            game.GoToMerchant(destination);
                            Console.WriteLine($"Welcome to {game.CurrentMerchant.Name}");
                            continue;
                        }
                        catch (InvalidMerchantException)
                        {
                            // do nothing. try other command.
                        }
                        catch (SameMerchantException)
                        {
                            Console.WriteLine("Same merchant as current");
                            continue;
                        }
                        catch (MerchantNotInTownException)
                        {
                            // do nothing. try other command.
                        }

                        Console.WriteLine("Invalid location");
                    }
                    else if (command.Equals("attack", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var monster = game.CurrentMonster;
                            game.Attack();
                            if (game.CurrentMonster == monster)
                            {
                                Console.WriteLine($"{game.CurrentMonster.Name}'s health is {game.CurrentMonster.Health}");

                                if (game.Player.Health == 0)
                                {
                                    //todo event
                                    Console.WriteLine("You are defeated");
                                }
                                else
                                {
                                    Console.WriteLine($"Your health is {game.Player.Health}");
                                }
                            }
                            else
                            {
                                //todo event

                                if (game.CurrentMonster != null)
                                {
                                    //todo event
                                    Console.WriteLine($"You encounter a {game.CurrentMonster.Name}");
                                }
                                else
                                {
                                    Console.WriteLine($"Welcome in {game.CurrentTown.Name}");
                                }
                            }
                        }
                        catch (NotInBattleModeException)
                        {
                            Console.WriteLine("You are not in a battle");
                        }
                    }
                    else if (command.Equals("inventory", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ShowInventory(game);
                    }
                    else if (command.Equals("requests", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (game.CurrentMerchant != null)
                        {
                            ShwoRequests(game.CurrentMerchant);
                        }
                        else
                        {
                            Console.WriteLine("Not at a merchant");
                        }
                    }
                    else if (command.Equals("offers", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (game.CurrentMerchant != null)
                        {
                            ShwoOffers(game.CurrentMerchant);
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
                        else if (game.CurrentMerchant == null)
                        {
                            Console.WriteLine("Not at a merchant");
                        }
                        else if (!game.Player.Inventory.Contains(item.Id))
                        {
                            Console.WriteLine("You does not have item");
                        }
                        else if (!game.CurrentMerchant.Requests.Where(t => t.ItemId == item.Id).Any())
                        {
                            Console.WriteLine("Merchant does not request item");
                        }
                        else
                        {
                            var value = game.CurrentMerchant.Requests.Where(t => t.ItemId == item.Id).Single().Price;
                            game.Player.Inventory.Remove(item.Id);
                            game.Player.Coins += value;
                            Console.WriteLine($"Item sold. You have now {game.Player.Coins}c");
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
                        else if (game.CurrentMerchant == null)
                        {
                            Console.WriteLine("Not at a merchant");
                        }
                        else if (!game.CurrentMerchant.Offers.Where(t => t.ItemId == item.Id).Any())
                        {
                            Console.WriteLine("Merchant does not offer item");
                        }
                        else if (game.Player.Coins < game.CurrentMerchant.Offers.Where(t => t.ItemId == item.Id).Single().Price)
                        {
                            Console.WriteLine("You don't have enough coins");
                        }
                        else
                        {
                            var value = game.CurrentMerchant.Offers.Where(t => t.ItemId == item.Id).Single().Price;
                            game.Player.Inventory.Add(item.Id);
                            game.Player.Coins -= value;
                            Console.WriteLine($"Bought item. You have now {game.Player.Coins}c");
                        }
                    }
                    else if (command.StartsWith("use ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var itemName = command.Substring(4);
                        var item = items.Where(t => t.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        if (item == null)
                        {
                            Console.WriteLine("Invalid item");
                        }
                        else if (!game.Player.Inventory.Contains(item.Id))
                        {
                            Console.WriteLine("You don't have this item");
                        }
                        else if (item is HealthPotion hp)
                        {
                            game.Player.Inventory.Remove(item.Id);
                            game.Player.Health = Math.Min(game.Player.MaxHealth, game.Player.Health + hp.Health);
                            Console.WriteLine($"You have {game.Player.Health} health");
                        }
                    }
                }
                catch (InBattleModeException)
                {
                    Console.WriteLine("You can't do this in battle");
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

        private static Item CreateHealthPotion(string name, int health)
        {
            var item = new HealthPotion
            {
                Id = Guid.NewGuid(),
                Name = name,
                Health = health
            };

            return item;
        }

        private static void MonsterDefeated(object sender, MonsterDefeatedEventArgs e)
        {
            Console.WriteLine($"{e.Monster.Name} is defeated");
            if (e.Loot.Any())
            {
                Console.WriteLine($"Loot: {string.Join(", ", e.Loot.Select(t => t.Name))}");
            }
        }

        private static void ShowInventory(MonsterHuntGame game)
        {
            foreach (var itemId in game.Player.Inventory)
            {
                var item = FindItem(itemId);
                Console.WriteLine(item.Name);
            }
        }

        private static void ShwoRequests(Merchant merchant)
        {
            foreach (var request in merchant.Requests)
            {
                var item = FindItem(request.ItemId);
                Console.WriteLine($"{item.Name} ({request.Price}c)");
            }
        }

        private static void ShwoOffers(Merchant merchant)
        {
            foreach (var offer in merchant.Offers)
            {
                var item = FindItem(offer.ItemId);
                Console.WriteLine($"{item.Name} ({offer.Price}c)");
            }
        }

        private static Item FindItem(Guid itemId)
        {
            return items.Where(t => t.Id == itemId).First();
        }
    }
}
