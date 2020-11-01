using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class ElementRepository
    {
        private readonly List<Element> elements = new List<Element>();

        public void Add(Element element)
        {
            elements.Add(element);
        }

        public Element Get(Guid id)
        {
            var element = elements
                .Where(t => t.Id == id)
                .Single();

            return element;
        }
    }
}