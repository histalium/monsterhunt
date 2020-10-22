using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class MonsterHuntGame
    {
        private readonly Dice dice = new Dice();
        private readonly UnitOfWork unitOfWork;

        private IEnumerator<Monster> encounters;

        public event EventHandler<MonsterDefeatedEventArgs> MonsterDefeated;

        public event EventHandler<MonsterEncounteredEventArgs> MonsterEncountered;

        public event EventHandler<PlayerHealthChangedEventArgs> PlayerHealthChanged;

        public event EventHandler<MonsterHealthChangedEventArgs> MonsterHealthChanged;

        public event EventHandler PlayerDefeated;

        public event EventHandler<ArrivedAtLocationEventArgs> ArrivedAtLocation;

        public MonsterHuntGame(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            CurrentTown = unitOfWork.Towns.Find("town 1");
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

            var town = unitOfWork.Towns.Find(townName);

            if (town == null)
            {
                throw new InvalidTownException();
            }

            if (town == CurrentTown)
            {
                throw new SameTownException();
            }

            var route = unitOfWork.Routes.FindBetweenTowns(CurrentTown.Id, town.Id);

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
            var monster = unitOfWork.Monsters.Get(monsterId);

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
            var weaponAttack = Player.WeaponId.HasValue ? ((Weapon)unitOfWork.Items.Get(Player.WeaponId.Value)).Attack : 0;
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
                    ArrivedAtLocation?.Invoke(this, new ArrivedAtLocationEventArgs(CurrentTown));
                }
                else
                {
                    MonsterEncountered?.Invoke(this, new MonsterEncounteredEventArgs(CurrentMonster));
                }

                return;
            }
            else
            {
                MonsterHealthChanged?.Invoke(this, new MonsterHealthChangedEventArgs(CurrentMonster.Health));
            }

            var attackRollMonster = dice.Roll();
            var legDefencePlayer = Player.LegArmorId.HasValue ? ((LegArmor)unitOfWork.Items.Get(Player.LegArmorId.Value)).Defence : 0;
            var bodyDefencePlayer = Player.BodyArmorId.HasValue ? ((BodyArmor)unitOfWork.Items.Get(Player.BodyArmorId.Value)).Defence : 0;
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

            if (Player.Health <= 0)
            {
                PlayerDefeated.Invoke(this, EventArgs.Empty);
            }
            else if (attackMonster != 0)
            {
                PlayerHealthChanged?.Invoke(this, new PlayerHealthChangedEventArgs(Player.Health));
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

            var merchant = unitOfWork.Merchants.Find(merchantName);

            if (merchant == null)
            {
                throw new InvalidMerchantException();
            }

            if (merchant == CurrentMerchant)
            {
                throw new SameMerchantException();
            }

            if (merchant.Town != CurrentTown.Id)
            {
                throw new MerchantNotInTownException();
            }

            CurrentMerchant = merchant;
        }

        private List<Item> GetLoot()
        {
            var lootDice = dice.Roll();
            var lootId = CurrentMonster.Loot.GetResult(lootDice);

            if (!lootId.HasValue)
            {
                return new List<Item>();
            }

            var loot = unitOfWork.Items.Get(lootId.Value);
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

            var item = unitOfWork.Items.Find(weaponName);

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

            var item = unitOfWork.Items.Find(bodyArmorName);

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

            var item = unitOfWork.Items.Find(legArmorName);

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

        internal IReadOnlyCollection<Item> GetInventory()
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            var inventory = Player.Inventory
                .Select(t => unitOfWork.Items.Get(t))
                .ToList();

            return inventory.AsReadOnly();
        }

        internal IReadOnlyCollection<(Item Item, int Price)> GetRequests()
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (CurrentMerchant == null)
            {
                throw new NotAtAMerchantException();
            }

            var requests = CurrentMerchant.Requests
                .Select(t => (unitOfWork.Items.Get(t.Item), t.Price))
                .ToList();

            return requests.AsReadOnly();
        }

        internal IReadOnlyCollection<(Item Item, int Price)> GetOffers()
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            if (CurrentMerchant == null)
            {
                throw new NotAtAMerchantException();
            }

            var offers = CurrentMerchant.Offers
                .Select(t => (unitOfWork.Items.Get(t.Item), t.Price))
                .ToList();

            return offers.AsReadOnly();
        }

        private bool HasItem(Item item)
        {
            var hasItem = Player.Inventory
                .Where(t => t == item.Id)
                .Any();

            return hasItem;
        }

        internal void UseItem(string itemName)
        {
            if (Player.Health <= 0)
            {
                throw new DefeatedException();
            }

            var item = unitOfWork.Items.Find(itemName);

            if (item == null)
            {
                throw new InvalidItemException();
            }
            else if (!Player.Inventory.Contains(item.Id))
            {
                throw new DoNotOwnItemException();
            }
            else if (item is HealthPotion hp)
            {
                Player.Inventory.Remove(item.Id);
                Player.Health = Math.Min(Player.MaxHealth, Player.Health + hp.Health);
                PlayerHealthChanged?.Invoke(this, new PlayerHealthChangedEventArgs(Player.Health));
            }
            else
            {
                throw new CanNotUseItemException();
            }
        }

        internal void BuyItem(string itemName)
        {
            var item = unitOfWork.Items.Find(itemName);
            if (item == null)
            {
                throw new InvalidItemException();
            }
            else if (CurrentMerchant == null)
            {
                throw new NotAtAMerchantException();
            }
            else if (!CurrentMerchant.Offers.Where(t => t.Item == item.Id).Any())
            {
                throw new MerchantDoesNotOfferException();
            }
            else if (Player.Coins < CurrentMerchant.Offers.Where(t => t.Item == item.Id).Single().Price)
            {
                throw new NotEnoughCoinsException();
            }
            else
            {
                var price = CurrentMerchant.Offers.Where(t => t.Item == item.Id).Single().Price;
                Player.Inventory.Add(item.Id);
                Player.Coins -= price;
            }
        }

        internal void SellItem(string itemName)
        {
            var item = unitOfWork.Items.Find(itemName);
            if (item == null)
            {
                throw new InvalidItemException();
            }
            else if (CurrentMerchant == null)
            {
                throw new NotAtAMerchantException();
            }
            else if (!Player.Inventory.Contains(item.Id))
            {
                throw new DoNotOwnItemException();
            }
            else if (!CurrentMerchant.Requests.Where(t => t.Item == item.Id).Any())
            {
                throw new MerchantDoesNotRequestException();
            }
            else
            {
                var value = CurrentMerchant.Requests.Where(t => t.Item == item.Id).Single().Price;
                Player.Inventory.Remove(item.Id);
                Player.Coins += value;
            }
        }
    }
}