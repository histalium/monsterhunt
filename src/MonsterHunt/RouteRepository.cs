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

        public Route FindBetweenTowns(Guid startingPoint, Guid destination)
        {
            var route = routes
                .Where(t => t.StartingPoint == startingPoint && t.Destination == destination)
                .SingleOrDefault();

            return route;
        }
    }
}