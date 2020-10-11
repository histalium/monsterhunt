using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Player
    {
        internal Guid Id { get; set; }

        internal int Attack { get; set; }

        internal int Defense { get; set; }

        internal int Health { get; set; }

        internal int MaxHealth { get; set; }

        internal List<Guid> Inventory { get; set; }

        internal int Coins { get; set; }
    }
}