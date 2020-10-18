﻿using System;
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
            var item5 = CreateWeapon("Weapon 1", 1);
            var item6 = CreateBodyArmor("Body armor 1", 1);
            var item7 = CreateLegArmor("Leg armor 1", 1);

            items = new List<Item> { item1, item2, item3, item4, item5, item6, item7 };

            var monster1 = new Monster
            {
                Id = Guid.NewGuid(),
                Name = "Monster 1",
                Attack = 0,
                Defense = 1,
                Health = 15,
                Loot = new RollResult()
                    .Set(1, 2, item1.Id)
                    .Set(3, item2.Id)
            };

            var monster2 = new Monster
            {
                Id = Guid.NewGuid(),
                Name = "Monster 2",
                Attack = 1,
                Defense = 1,
                Health = 12,
                Loot = new RollResult()
                    .Set(1, 2, item1.Id)
                    .Set(3, item3.Id)
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
                        ItemId = item2.Id,
                        Price = 2
                    }
                },
                Offers = new List<ItemPrice>
                {
                    new ItemPrice
                    {
                        ItemId = item4.Id,
                        Price = 5
                    },
                    new ItemPrice
                    {
                        ItemId = item5.Id,
                        Price = 7
                    },
                    new ItemPrice
                    {
                        ItemId = item6.Id,
                        Price = 7
                    },
                    new ItemPrice
                    {
                        ItemId = item7.Id,
                        Price = 7
                    }
                }
            };

            var merchant2 = new Merchant
            {
                Name = "Merchant 2",
                TownId = town2.Id,
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
                        Price = 3
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

            merchants = new List<Merchant> { merchant1, merchant2 };

            var game = new MonsterHuntGame(towns, routes, monsters, items, merchants);
            game.MonsterDefeated += MonsterDefeated;
            game.MonsterEncountered += MonsterEncountered;

            Console.WriteLine($"Welcome in {game.CurrentTown.Name}");

            Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();
            commands.Add("go to ", GetGoToCommand(game));
            commands.Add("attack", GeAttackCommand(game));
            commands.Add("inventory", GetInventoryCommand(game));
            commands.Add("requests", GetRequestsCommand(game));
            commands.Add("offers", GetOffersCommand(game));
            commands.Add("sell ", GetSellCommand(game));
            commands.Add("buy ", GetBuyCommand(game));
            commands.Add("use ", GetUseCommand(game));
            commands.Add("equip ", GetEquipCommand(game));

            foreach (var command in ReadLines())
            {
                if (command.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                try
                {
                    var c = commands
                        .Where(t => command.StartsWith(t.Key, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                    if (c.Key != null)
                    {
                        c.Value(command.Substring(c.Key.Length));
                    }
                    else
                    {
                        Console.WriteLine("Unknown command");
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

        private static Item CreateWeapon(string name, int attack)
        {
            var item = new Weapon
            {
                Id = Guid.NewGuid(),
                Name = name,
                Attack = attack
            };

            return item;
        }

        private static Item CreateBodyArmor(string name, int defence)
        {
            var item = new BodyArmor
            {
                Id = Guid.NewGuid(),
                Name = name,
                Defence = defence
            };

            return item;
        }

        private static Item CreateLegArmor(string name, int defence)
        {
            var item = new LegArmor
            {
                Id = Guid.NewGuid(),
                Name = name,
                Defence = defence
            };

            return item;
        }

        private static Action<string> GetGoToCommand(MonsterHuntGame game)
        {
            Action<string> command = (destination) =>
            {
                try
                {
                    game.GoToTown(destination);
                    return;
                }
                catch (InvalidTownException)
                {
                    // do nothing. try other command.
                }
                catch (SameTownException)
                {
                    Console.WriteLine("Same town as current");
                    return;
                }
                catch (NoRouteToTownException)
                {
                    Console.WriteLine("No route to town");
                }

                try
                {
                    game.GoToMerchant(destination);
                    Console.WriteLine($"Welcome to {game.CurrentMerchant.Name}");
                    return;
                }
                catch (InvalidMerchantException)
                {
                    // do nothing. try other command.
                }
                catch (SameMerchantException)
                {
                    Console.WriteLine("Same merchant as current");
                    return;
                }
                catch (MerchantNotInTownException)
                {
                    // do nothing. try other command.
                }

                Console.WriteLine("Invalid location");
            };

            return command;
        }

        private static Action<string> GeAttackCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
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

                        if (game.CurrentMonster == null)
                        {
                            Console.WriteLine($"Welcome in {game.CurrentTown.Name}");
                        }
                    }
                }
                catch (NotInBattleModeException)
                {
                    Console.WriteLine("You are not in a battle");
                }
            };

            return command;
        }

        private static void MonsterDefeated(object sender, MonsterDefeatedEventArgs e)
        {
            Console.WriteLine($"{e.Monster.Name} is defeated");
            if (e.Loot.Any())
            {
                Console.WriteLine($"Loot: {string.Join(", ", e.Loot.Select(t => t.Name))}");
            }
        }

        private static void MonsterEncountered(object sender, MonsterEncounteredEventArgs e)
        {
            Console.WriteLine($"You encounter a {e.Monster.Name}");
        }

        private static Action<string> GetInventoryCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
            {
                foreach (var itemId in game.Player.Inventory)
                {
                    var item = FindItem(itemId);
                    Console.WriteLine(item.Name);
                }
            };

            return command;
        }

        private static Action<string> GetRequestsCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
            {
                if (game.CurrentMerchant != null)
                {
                    var merchant = game.CurrentMerchant;
                    foreach (var request in merchant.Requests)
                    {
                        var item = FindItem(request.ItemId);
                        Console.WriteLine($"{item.Name} ({request.Price}c)");
                    }
                }
                else
                {
                    Console.WriteLine("Not at a merchant");
                }
            };

            return command;
        }

        private static Action<string> GetOffersCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
            {
                if (game.CurrentMerchant != null)
                {
                    var merchant = game.CurrentMerchant;
                    foreach (var offer in merchant.Offers)
                    {
                        var item = FindItem(offer.ItemId);
                        Console.WriteLine($"{item.Name} ({offer.Price}c)");
                    }
                }
                else
                {
                    Console.WriteLine("Not at a merchant");
                }
            };

            return command;
        }

        private static Item FindItem(Guid itemId)
        {
            return items.Where(t => t.Id == itemId).First();
        }

        private static Action<string> GetSellCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
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
            };

            return command;
        }

        private static Action<string> GetBuyCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
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
            };

            return command;
        }

        private static Action<string> GetUseCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
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
            };

            return command;
        }

        private static Action<string> GetEquipCommand(MonsterHuntGame game)
        {
            Action<string> command = (item) =>
            {
                try
                {
                    game.EquipWeapon(item);
                    return;
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                    return;
                }
                catch (ItemNotAWeaponException)
                {
                    // do nothing. try other command.
                }
                catch (DoNotOwnItemException)
                {
                    Console.WriteLine("You don't have this item");
                    return;
                }

                try
                {
                    game.EquipBodyArmor(item);
                    return;
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                    return;
                }
                catch (ItemNotBodyArmorException)
                {
                    // do nothing. try other command.
                }
                catch (DoNotOwnItemException)
                {
                    Console.WriteLine("You don't have this item");
                    return;
                }

                try
                {
                    game.EquipLegArmor(item);
                    return;
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                    return;
                }
                catch (ItemNotLegArmorException)
                {
                    // do nothing. try other command.
                }
                catch (DoNotOwnItemException)
                {
                    Console.WriteLine("You don't have this item");
                    return;
                }

                Console.WriteLine("Can't equip item");
            };

            return command;
        }
    }
}
