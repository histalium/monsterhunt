using System;

namespace MonsterHunt
{
    internal class Route
    {
        public Guid Id { get; set; }

        public Town Destination { get; set; }

        public int NumberOfMonsters { get; set; }

        public Monster[] Monsters { get; set; }
    }
}