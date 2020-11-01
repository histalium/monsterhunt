using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Route
    {
        public Guid Id { get; set; }

        public Guid StartingPoint { get; set; }

        public Guid Destination { get; set; }

        public int NumberOfMonsters { get; set; }

        public RollResult Monsters { get; set; }
    }
}