using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    internal class GameData
    {
        public List<Item> Items { get; set; }
        public List<Town> Towns { get; set; }
        public List<Monster> Monsters { get; set; }
        public List<Route> Routes { get; set; }
    }
}