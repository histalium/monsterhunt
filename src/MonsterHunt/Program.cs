using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MonsterHunt
{
    class Program
    {
        private static List<Monster> monsters;
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

            var items = new ItemRepository();
            items.Add(item1);
            items.Add(item2);
            items.Add(item3);
            items.Add(item4);
            items.Add(item5);
            items.Add(item6);
            items.Add(item7);

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

            var towns = new TownRepository();
            towns.Add(town1);
            towns.Add(town2);

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
            game.PlayerHealthChanged += PlayerHealthChanged;
            game.PlayerDefeated += PlayerDefeated;
            game.MonsterHealthChanged += MonsterHealthChanged;
            game.ArrivedAtLocation += ArrivedAtLocation;

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
                catch (DefeatedException)
                {
                    Console.WriteLine("You are defeated");
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
                    game.Attack();
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

        private static void PlayerHealthChanged(object sender, PlayerHealthChangedEventArgs e)
        {
            Console.WriteLine($"Your health is {e.Health}");
        }

        private static void PlayerDefeated(object sender, EventArgs e)
        {
            Console.WriteLine("You are defeated");
        }

        private static void MonsterHealthChanged(object sender, MonsterHealthChangedEventArgs e)
        {
            Console.WriteLine($"Monster's health is {e.Health}");
        }

        private static void ArrivedAtLocation(object sender, ArrivedAtLocationEventArgs e)
        {
            Console.WriteLine($"Welcome in {e.Town.Name}");
        }

        private static Action<string> GetInventoryCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
            {
                var inventory = game.GetInventory();
                foreach (var item in inventory)
                {
                    Console.WriteLine(item.Name);
                }
            };

            return command;
        }

        private static Action<string> GetRequestsCommand(MonsterHuntGame game)
        {
            Action<string> command = (v) =>
            {
                try
                {
                    var requests = game.GetRequests();
                    foreach (var (item, price) in requests)
                    {
                        Console.WriteLine($"{item.Name} ({price}c)");
                    }
                }
                catch (NotAtAMerchantException)
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
                try
                {
                    var requests = game.GetOffers();
                    foreach (var (item, price) in requests)
                    {
                        Console.WriteLine($"{item.Name} ({price}c)");
                    }
                }
                catch (NotAtAMerchantException)
                {
                    Console.WriteLine("Not at a merchant");
                }
            };

            return command;
        }

        private static Action<string> GetSellCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
                try
                {
                    game.SellItem(itemName);
                    Console.WriteLine($"Item sold. You have now {game.Player.Coins}c");
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                }
                catch (NotAtAMerchantException)
                {
                    Console.WriteLine("Not at a merchant");
                }
                catch (MerchantDoesNotRequestException)
                {
                    Console.WriteLine("Merchant does not request item");
                }
                catch (NotEnoughCoinsException)
                {
                    Console.WriteLine("You don't have enough coins");
                }
                catch (DoNotOwnItemException)
                {
                    Console.WriteLine("You do not have this item");
                }
            };

            return command;
        }

        private static Action<string> GetBuyCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
                try
                {
                    game.BuyItem(itemName);
                    Console.WriteLine($"Bought item. You have now {game.Player.Coins}c");
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                }
                catch (NotAtAMerchantException)
                {
                    Console.WriteLine("Not at a merchant");
                }
                catch (MerchantDoesNotOfferException)
                {
                    Console.WriteLine("Merchant does not offer item");
                }
                catch (NotEnoughCoinsException)
                {
                    Console.WriteLine("You don't have enough coins");
                }
            };

            return command;
        }

        private static Action<string> GetUseCommand(MonsterHuntGame game)
        {
            Action<string> command = (itemName) =>
            {
                try
                {
                    game.UseItem(itemName);
                }
                catch (InvalidItemException)
                {
                    Console.WriteLine("Invalid item");
                }
                catch (DoNotOwnItemException)
                {
                    Console.WriteLine("You do not have this item");
                }
                catch (CanNotUseItemException)
                {
                    Console.WriteLine("Iten can't be used");
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
