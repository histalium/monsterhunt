using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class TownRepository
    {
        private readonly List<Town> towns = new List<Town>();

        public void Add(Town town)
        {
            towns.Add(town);
        }

        public Town Find(string townName)
        {
            var town = towns
                .Where(t => t.Name.AreEqualIgnoreCase(townName))
                .SingleOrDefault();

            return town;
        }
    }
}