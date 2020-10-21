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
    }
}