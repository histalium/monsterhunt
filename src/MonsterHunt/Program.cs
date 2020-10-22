using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace MonsterHunt
{
    class Program
    {
        static void Main(string[] args)
        {
            var unitOfWork = LoadGameData();

            var game = new MonsterHuntGame(unitOfWork);
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

        private static UnitOfWork LoadGameData()
        {
            using var fs = new FileStream("gameData.json", FileMode.Open);
            var gameData = GameDataLoader.Load(fs);

            return gameData;
        }

        private static IEnumerable<string> ReadLines()
        {
            while (true)
            {
                yield return Console.ReadLine();
            }
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
