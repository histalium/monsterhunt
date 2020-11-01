using System;
using System.Collections.Generic;

namespace MonsterHunt.JsonFileGameData
{
    internal class Merchant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Town { get; set; }

        public List<ItemPrice> Requests { get; set; }

        public List<ItemPrice> Offers { get; set; }
    }
}