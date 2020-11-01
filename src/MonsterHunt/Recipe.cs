using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Recipe : Item
    {
        public List<Guid> Ingredients { get; set; }

        public List<Guid> Results { get; set; }
    }
}