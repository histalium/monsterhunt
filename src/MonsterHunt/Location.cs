using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Location
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Guid> ConnectedLocations { get; set; }
    }
}