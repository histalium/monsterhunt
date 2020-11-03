using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    internal class GameData
    {
        public List<Element> Elements { get; set; }
        public List<Item> Items { get; set; }
        public List<Town> Towns { get; set; }
        public List<Monster> Monsters { get; set; }
        public List<Route> Routes { get; set; }
        public List<Merchant> Merchants { get; set; }
        public List<Location> Locations { get; set; }
    }
}