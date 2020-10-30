using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Stats
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public string WeaponName { get; set; }
        public int WeaponAttack { get; set; }
        public List<ElementAttack> ElementAttacks { get; set; }
        public string BodyArmorName { get; set; }
        public int BodyArmorDefence { get; set; }
        public string LegArmorName { get; set; }
        public int LegArmorDefence { get; set; }
    }
}