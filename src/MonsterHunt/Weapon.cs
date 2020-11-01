using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Weapon : Item
    {
        public int Attack { get; set; }

        public List<ElementAttack> ElementAttacks { get; set; }
    }
}
