using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class MonsterHuntGame
    {
        private readonly TownRepository towns;
        private readonly List<Route> routes;
        private readonly List<Monster> monsters;
        private readonly List<Item> items;
        private readonly List<Merchant> merchants;

        private readonly Dice dice = new Dice();

        private IEnumerator<Monster> encounters;

        public event EventHandler<MonsterDefeatedEventArgs> MonsterDefeated;

        public event EventHandler<MonsterEncounteredEventArgs> MonsterEncountered;

        public MonsterHuntGame(TownRepository towns, List<Route> routes, List<Monster> monsters, List<Item> items,
            List<Merchant> merchants)
        {
            this.towns = towns;
            this.routes = routes;
            this.monsters = monsters;
            this.items = items;
            this.merchants = merchants;
            CurrentTown = towns.Find("town 1");
            Player = CreatePlayer();
        }

        public Town CurrentTown { get; private set; }

        public Monster CurrentMonster { get; private set; }

        public Merchant CurrentMerchant { get; private set; }

        public Player Player { get; }

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
                Coins = 50
            };

            return player;
        }

        public void GoToTown(string townName)
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var town = towns.Find(townName);

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

            if (CurrentMonster != null)
            {
                MonsterEncountered?.Invoke(this, new MonsterEncounteredEventArgs(CurrentMonster));
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
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (CurrentMonster == null)
            {
                throw new NotInBattleModeException();
            }

            var attackRollPlayer = dice.Roll();
            var weaponAttack = Player.WeaponId.HasValue ? ((Weapon)GetItem(Player.WeaponId.Value)).Attack : 0;
            var attackPlayer = Player.Attack + attackRollPlayer + weaponAttack - CurrentMonster.Defense;
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
                foreach (var item in loot)
                {
                    Player.Inventory.Add(item.Id);
                }

                MonsterDefeated?.Invoke(this, new MonsterDefeatedEventArgs(CurrentMonster, loot));

                CurrentMonster = encounters.Next();
                if (CurrentMonster == null)
                {
                    encounters = null;
                }
                else
                {
                    MonsterEncountered?.Invoke(this, new MonsterEncounteredEventArgs(CurrentMonster));
                }

                return;
            }

            var attackRollMonster = dice.Roll();
            var legDefencePlayer = Player.LegArmorId.HasValue ? ((LegArmor)GetItem(Player.LegArmorId.Value)).Defence : 0;
            var bodyDefencePlayer = Player.BodyArmorId.HasValue ? ((BodyArmor)GetItem(Player.BodyArmorId.Value)).Defence : 0;
            var attackMonster = CurrentMonster.Attack + attackRollMonster - Player.Defense - legDefencePlayer - bodyDefencePlayer;
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
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

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

        private List<Item> GetLoot()
        {
            var lootDice = dice.Roll();
            var lootId = CurrentMonster.Loot.GetResult(lootDice);

            if (!lootId.HasValue)
            {
                return new List<Item>();
            }

            var loot = items.Where(t => t.Id == lootId).Single();
            return new List<Item> { loot };
        }

        internal void EquipWeapon(string weaponName)
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var item = FindItem(weaponName);

            if (item == null)
            {
                throw new InvalidItemException();
            }

            var weapon = item as Weapon;

            if (weapon == null)
            {
                throw new ItemNotAWeaponException();
            }

            if (!HasItem(weapon))
            {
                throw new DoNotOwnItemException();
            }

            if (Player.WeaponId.HasValue)
            {
                Player.Inventory.Add(Player.WeaponId.Value);
            }

            Player.Inventory.Remove(weapon.Id);
            Player.WeaponId = weapon.Id;
        }

        internal void EquipBodyArmor(string bodyArmorName)
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var item = FindItem(bodyArmorName);

            if (item == null)
            {
                throw new InvalidItemException();
            }

            var armor = item as BodyArmor;

            if (armor == null)
            {
                throw new ItemNotBodyArmorException();
            }

            if (!HasItem(armor))
            {
                throw new DoNotOwnItemException();
            }

            if (Player.BodyArmorId.HasValue)
            {
                Player.Inventory.Add(Player.BodyArmorId.Value);
            }

            Player.Inventory.Remove(armor.Id);
            Player.BodyArmorId = armor.Id;
        }

        internal void EquipLegArmor(string legArmorName)
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (encounters != null)
            {
                throw new InBattleModeException();
            }

            var item = FindItem(legArmorName);

            if (item == null)
            {
                throw new InvalidItemException();
            }

            var armor = item as LegArmor;

            if (armor == null)
            {
                throw new ItemNotLegArmorException();
            }

            if (!HasItem(armor))
            {
                throw new DoNotOwnItemException();
            }

            if (Player.LegArmorId.HasValue)
            {
                Player.Inventory.Add(Player.LegArmorId.Value);
            }

            Player.Inventory.Remove(armor.Id);
            Player.LegArmorId = armor.Id;
        }

        private Item FindItem(string name)
        {
            var item = items
                .Where(t => AreEqual(name, t.Name))
                .FirstOrDefault();

            return item;
        }

        private bool HasItem(Item item)
        {
            var hasItem = Player.Inventory
                .Where(t => t == item.Id)
                .Any();

            return hasItem;
        }

        private Item GetItem(Guid itemId)
        {
            var item = items
                .Where(t => t.Id == itemId)
                .Single();

            return item;
        }
    }
}