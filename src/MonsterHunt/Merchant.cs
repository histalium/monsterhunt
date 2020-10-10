using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Merchant
    {
        public string Name { get; set; }

        public Guid TownId { get; set; }

        public List<ItemPrice> Requests { get; set; }

        public List<ItemPrice> Offers { get; set; }
    }
}