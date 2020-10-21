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

    public class MonsterEncounteredEventArgs : EventArgs
    {
        internal MonsterEncounteredEventArgs(Monster monster)
        {
            Monster = monster;
        }

        internal Monster Monster { get; }
    }

    public class PlayerHealthChangedEventArgs : EventArgs
    {
        internal PlayerHealthChangedEventArgs(int health)
        {
            Health = health;
        }

        internal int Health { get; }
    }

    public class MonsterHealthChangedEventArgs : EventArgs
    {
        internal MonsterHealthChangedEventArgs(int health)
        {
            Health = health;
        }

        internal int Health { get; }
    }
}