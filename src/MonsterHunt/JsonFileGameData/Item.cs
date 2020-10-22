using System;

namespace MonsterHunt.JsonFileGameData
{
    internal class Item
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
    }
}