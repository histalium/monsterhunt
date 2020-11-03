using System;
using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> ConnectedLocations { get; set; }
    }
}