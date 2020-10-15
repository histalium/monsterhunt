using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    public class MonsterDefeatedEventArgs : EventArgs
    {
        internal MonsterDefeatedEventArgs(Monster monster, IReadOnlyList<Item> loot)
        {
            Monster = monster;
            Loot = loot;
        }

        internal Monster Monster { get; }

        internal IReadOnlyList<Item> Loot { get; }
    }
}