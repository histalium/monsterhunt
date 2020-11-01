using System;
using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    internal class Monster
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Health { get; set; }
        public RollResult Loot { get; set; }
        public List<ElementDefense> ElementDefenses { get; set; }
    }
}