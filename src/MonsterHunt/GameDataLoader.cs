using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MonsterHunt
{
    internal class GameDataLoader
    {
        public static UnitOfWork Load(Stream file)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var sr = new StreamReader(file);
            var fileContent = sr.ReadToEnd();

            var fileData = JsonSerializer.Deserialize<JsonFileGameData.GameData>(fileContent, jsonOptions);

            var unitOfWork = new UnitOfWork();

            const string itemType = "item";
            const string healthPotionType = "health potion";
            const string weaponType = "weapon";
            const string bodyArmorType = "body armor";
            const string legArmorType = "leg armor";

            foreach (var itemData in fileData.Items)
            {
                switch (itemData.Type)
                {
                    case itemType:
                        var item = CreateItem(itemData);
                        unitOfWork.Items.Add(item);
                        break;
                    case healthPotionType:
                        var healthPotion = CreateHealthPotion(itemData);
                        unitOfWork.Items.Add(healthPotion);
                        break;
                    case weaponType:
                        var weapon = CreateWeapon(itemData);
                        unitOfWork.Items.Add(weapon);
                        break;
                    case bodyArmorType:
                        var bodyArmor = CreateBodyArmor(itemData);
                        unitOfWork.Items.Add(bodyArmor);
                        break;
                    case legArmorType:
                        var legArmor = CreateLegArmor(itemData);
                        unitOfWork.Items.Add(legArmor);
                        break;
                }
            }

            foreach (var townData in fileData.Towns)
            {
                var town = CreateTown(townData);
                unitOfWork.Towns.Add(town);
            }

            foreach (var monsterData in fileData.Monsters)
            {
                var monster = CreateMonster(monsterData);
                unitOfWork.Monsters.Add(monster);
            }

            foreach (var routeData in fileData.Routes)
            {
                var route = CreateRoute(routeData);
                unitOfWork.Routes.Add(route);
            }

            foreach (var merchantData in fileData.Merchants)
            {
                var merchant = CreateMerchant(merchantData);
                unitOfWork.Merchants.Add(merchant);
            }

            return unitOfWork;
        }

        private static Item CreateItem(JsonFileGameData.Item itemData)
        {
            var item = new Item
            {
                Id = itemData.Id,
                Name = itemData.Name
            };

            return item;
        }

        public static HealthPotion CreateHealthPotion(JsonFileGameData.Item itemData)
        {
            var item = new HealthPotion
            {
                Id = itemData.Id,
                Name = itemData.Name,
                Health = itemData.Health
            };

            return item;
        }

        public static Weapon CreateWeapon(JsonFileGameData.Item itemData)
        {
            var item = new Weapon
            {
                Id = itemData.Id,
                Name = itemData.Name,
                Attack = itemData.Attack
            };

            return item;
        }

        public static BodyArmor CreateBodyArmor(JsonFileGameData.Item itemData)
        {
            var item = new BodyArmor
            {
                Id = itemData.Id,
                Name = itemData.Name,
                Defence = itemData.Defence
            };

            return item;
        }

        public static LegArmor CreateLegArmor(JsonFileGameData.Item itemData)
        {
            var item = new LegArmor
            {
                Id = itemData.Id,
                Name = itemData.Name,
                Defence = itemData.Defence
            };

            return item;
        }

        private static Town CreateTown(JsonFileGameData.Town townData)
        {
            var town = new Town
            {
                Id = townData.Id,
                Name = townData.Name
            };

            return town;
        }

        private static Monster CreateMonster(JsonFileGameData.Monster monsterData)
        {
            var monster = new Monster
            {
                Id = monsterData.Id,
                Name = monsterData.Name,
                Attack = monsterData.Attack,
                Defense = monsterData.Defense,
                Health = monsterData.Health,
                Loot = new RollResult()
                    .Set(1, GetRollResultId(monsterData.Loot?.Roll1))
                    .Set(2, GetRollResultId(monsterData.Loot?.Roll2))
                    .Set(3, GetRollResultId(monsterData.Loot?.Roll3))
                    .Set(4, GetRollResultId(monsterData.Loot?.Roll4))
                    .Set(5, GetRollResultId(monsterData.Loot?.Roll5))
                    .Set(6, GetRollResultId(monsterData.Loot?.Roll6))
            };

            return monster;
        }

        private static Route CreateRoute(JsonFileGameData.Route routeData)
        {
            var route = new Route
            {
                Id = routeData.Id,
                StartingPoint = routeData.StartingPoint,
                Destination = routeData.Destination,
                NumberOfMonsters = routeData.NumberOfMonsters,
                Monsters = new RollResult()
                    .Set(1, GetRollResultId(routeData.Monsters?.Roll1))
                    .Set(2, GetRollResultId(routeData.Monsters?.Roll2))
                    .Set(3, GetRollResultId(routeData.Monsters?.Roll3))
                    .Set(4, GetRollResultId(routeData.Monsters?.Roll4))
                    .Set(5, GetRollResultId(routeData.Monsters?.Roll5))
                    .Set(6, GetRollResultId(routeData.Monsters?.Roll6))
            };

            return route;
        }

        private static Merchant CreateMerchant(JsonFileGameData.Merchant merchantData)
        {
            var merchant = new Merchant
            {
                Id = merchantData.Id,
                Name = merchantData.Name,
                Town = merchantData.Town,
                Offers = merchantData.Offers.Select(t => new ItemPrice { Item = t.Item, Price = t.Price }).ToList(),
                Requests = merchantData.Requests.Select(t => new ItemPrice { Item = t.Item, Price = t.Price }).ToList(),
            };

            return merchant;
        }

        private static Guid? GetRollResultId(Guid? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            if (id.Value == Guid.Empty)
            {
                return null;
            }

            return id.Value;
        }
    }
}