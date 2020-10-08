using System.Collections.Generic;

namespace MonsterHunt
{
    internal class Merchant
    {
        public string Name { get; set; }
        public List<ItemPrice> Requests { get; set; }
    }
}