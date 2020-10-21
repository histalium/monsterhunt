using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class RouteRepository
    {
        private readonly List<Route> routes = new List<Route>();

        public void Add(Route route)
        {
            routes.Add(route);
        }

        public Route FindBetweenTowns(Guid townId1, Guid townId2)
        {
            var route = routes
                .Where(t => t.Towns.Contains(townId1) && t.Towns.Contains(townId2))
                .SingleOrDefault();

            return route;
        }
    }
}