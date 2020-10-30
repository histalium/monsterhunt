using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Monster
    {
        internal Guid Id { get; set; }

        internal string Name { get; set; }

        internal int Attack { get; set; }

        internal int Defense { get; set; }

        internal int Health { get; set; }

        internal RollResult Loot { get; set; }

        internal List<ElementDefense> ElementDefenses { get; set; }
    }
}