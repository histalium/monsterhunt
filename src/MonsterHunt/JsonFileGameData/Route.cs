using System;
using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    internal class Route
    {
        public Guid Id { get; set; }
        public List<Guid> Towns { get; set; }
        public int NumberOfMonsters { get; set; }
        public RollResult Monsters { get; set; }
    }
}