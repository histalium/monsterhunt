using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class MonsterHuntGame
    {
        private readonly List<Town> towns;
        private readonly List<Route> routes;
        private readonly List<Monster> monsters;
        private readonly List<Item> items;
        private readonly List<Merchant> merchants;

        private readonly Dice dice = new Dice();

        private IEnumerator<Monster> encounters;

        public MonsterHuntGame(List<Town> towns, List<Route> routes, List<Monster> monsters, List<Item> items,
            List<Merchant> merchants)
        {
            this.towns = towns;
            this.routes = routes;
            this.monsters = monsters;
            this.items = items;
            this.merchants = merchants;
            CurrentTown = FindTown("town 1");
            Player = CreatePlayer();
        }

        public Town CurrentTown { get; private set; }

        public Monster CurrentMonster { get; private set; }

        public Merchant CurrentMerchant { get; private set; }

        public Player Player { get; }

        private Town FindTown(string name)
        {
            var town = towns
                .Where(t => AreEqual(t.Name, name))
                .SingleOrDefault();

            return town;
        }

        private static bool AreEqual(string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
        }

        private static Player CreatePlayer()
        {
            var player = new Player
            {
                Id = Guid.NewGuid(),
                Attack = 1,
                Defense = 2,
                Health = 30,
                MaxHealth = 30,
                Inventory = new List<Guid>(),
                Coins = 10
            };

            return player;
        }

        public void GoToTown(string townName)
        {
            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var town = FindTown(townName);

            if (town == null)
            {
                throw new InvalidTownException();
            }

            if (town == CurrentTown)
            {
                throw new SameTownException();
            }

            var route = FindRouteToTown(town);

            if (route == null)
            {
                throw new NoRouteToTownException();
            }

            encounters = BuildEncounters(route).GetEnumerator();
            CurrentTown = town;
            CurrentMonster = encounters.Next();
            CurrentMerchant = null;

            if (CurrentMonster == null)
            {
                encounters = null;
            }
        }

        private Route FindRouteToTown(Town town)
        {
            var route = routes
                .Where(t => t.Towns.Contains(town.Id) && t.Towns.Contains(CurrentTown.Id))
                .SingleOrDefault();

            return route;
        }

        private IEnumerable<Monster> BuildEncounters(Route route)
        {
            for (var i = 0; i < 3; i++)
            {
                var monsterId = route.Monsters.GetResult(dice.Roll());

                if (!monsterId.HasValue)
                {
                    continue;
                }

                var monster = CreateMonsterInstance(monsterId.Value);
                yield return monster;
            }
        }

        private Monster CreateMonsterInstance(Guid monsterId)
        {
            var monster = GetMonster(monsterId);

            var copy = new Monster
            {
                Id = monster.Id,
                Attack = monster.Attack,
                Defense = monster.Defense,
                Name = monster.Name,
                Health = monster.Health,
                Loot = monster.Loot
            };

            return copy;
        }

        private Monster GetMonster(Guid monsterId)
        {
            var monster = monsters
                .Where(t => t.Id == monsterId)
                .Single();

            return monster;
        }

        internal void Attack()
        {
            if (CurrentMonster == null)
            {
                throw new NotInBattleModeException();
            }

            var attackRollPlayer = dice.Roll();
            var attackPlayer = Player.Attack + attackRollPlayer - CurrentMonster.Defense;
            attackPlayer = Math.Max(0, attackPlayer);
            if (attackPlayer > CurrentMonster.Health)
            {
                CurrentMonster.Health = 0;
            }
            else
            {
                CurrentMonster.Health -= attackPlayer;
            }

            if (CurrentMonster.Health == 0)
            {
                var loot = GetLoot();
                if (loot != null)
                {
                    //todo event
                    Console.WriteLine($"loot: {loot.Name}");
                    Player.Inventory.Add(loot.Id);
                }

                CurrentMonster = encounters.Next();
                if (CurrentMonster == null)
                {
                    encounters = null;
                }

                return;
            }

            var attackRollMonster = dice.Roll();
            var attackMonster = CurrentMonster.Attack + attackRollMonster - Player.Defense;
            attackMonster = Math.Max(0, attackMonster);
            if (attackMonster > Player.Health)
            {
                Player.Health = 0;
            }
            else
            {
                Player.Health -= attackMonster;
            }
        }

        internal void GoToMerchant(string merchantName)
        {
            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var merchant = FindMerchant(merchantName);

            if (merchant == null)
            {
                throw new InvalidMerchantException();
            }

            if (merchant == CurrentMerchant)
            {
                throw new SameMerchantException();
            }

            if (merchant.TownId != CurrentTown.Id)
            {
                throw new MerchantNotInTownException();
            }

            CurrentMerchant = merchant;
        }

        private Merchant FindMerchant(string name)
        {
            var merchant = merchants
                .Where(t => AreEqual(t.Name, name))
                .SingleOrDefault();

            return merchant;
        }

        private Item GetLoot()
        {
            var lootDice = dice.Roll();
            var lootId = CurrentMonster.Loot.GetResult(lootDice);

            if (!lootId.HasValue)
            {
                return null;
            }

            var loot = items.Where(t => t.Id == lootId).Single();
            return loot;
        }
    }
}