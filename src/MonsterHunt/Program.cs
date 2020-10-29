using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonsterHunt.Commands;

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

            var commands = new Dictionary<string, ICommand>();
            commands.Add("go to ", new GoToCommand(game));
            commands.Add("attack", new AttackCommand(game));
            commands.Add("inventory", new InventoryCommand(game));
            commands.Add("requests", new RequestsCommand(game));
            commands.Add("offers", new OffersCommand(game));
            commands.Add("sell ", new SellCommand(game));
            commands.Add("buy ", new BuyCommand(game));
            commands.Add("use ", new UseCommand(game));
            commands.Add("equip ", new EquipCommand(game));
            commands.Add("stats", new StatsCommand(game));
            commands.Add("learn ", new LearnCommand(game));
            commands.Add("recipes", new RecipesCommand(game));
            commands.Add("make ", new MakeCommand(game));

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
                        c.Value.Execute(command.Substring(c.Key.Length));
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
    }
}
