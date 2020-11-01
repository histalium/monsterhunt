using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class MonsterRepository
    {
        private readonly List<Monster> monsters = new List<Monster>();

        public void Add(Monster monster)
        {
            monsters.Add(monster);
        }

        public Monster Get(Guid monsterId)
        {
            var monster = monsters
                .Where(t => t.Id == monsterId)
                .Single();

            return monster;
        }

        public Monster Find(string monsterName)
        {
            var monster = monsters
                .Where(t => t.Name.AreEqualIgnoreCase(monsterName))
                .SingleOrDefault();

            return monster;
        }
    }
}