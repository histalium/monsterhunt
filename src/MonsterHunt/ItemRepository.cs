using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class ItemRepository
    {
        private readonly List<Item> items = new List<Item>();

        public void Add(Item item)
        {
            items.Add(item);
        }

        public Item Get(Guid itemId)
        {
            var item = items
                .Where(t => t.Id == itemId)
                .Single();

            return item;
        }

        public Item Find(string itemName)
        {
            var item = items
                .Where(t => t.Name.AreEqualIgnoreCase(itemName))
                .SingleOrDefault();

            return item;
        }
    }
}