using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class LocationRepository
    {
        private readonly List<Location> locations = new List<Location>();

        public void Add(Location location)
        {
            locations.Add(location);
        }

        public Location Get(Guid id)
        {
            var location = locations
                .Where(t => t.Id == id)
                .Single();

            return location;
        }
    }
}