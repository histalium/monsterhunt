using System;
using System.IO;
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

            foreach (var itemData in fileData.Items)
            {
                switch (itemData.Type)
                {
                    case itemType:
                        var item = CreateItem(itemData);
                        unitOfWork.Items.Add(item);
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