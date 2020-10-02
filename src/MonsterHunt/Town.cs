using System;
using System.Collections.Immutable;

namespace MonsterHunt
{
    internal class Town
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ImmutableList<Route> Routes { get; set; }
    }
}