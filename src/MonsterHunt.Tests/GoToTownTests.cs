using System;
using Xunit;

namespace MonsterHunt.Tests
{
    public class GoToTownTests
    {
        [Fact]
        public void WhenDefeated_ThenThrowDefeatedException()
        {
            var player = new Player { Id = Guid.NewGuid(), Health = 0 };
            var unitOfWork = new UnitOfWork();
            var town1 = new Town { Id = Guid.NewGuid(), Name = "Town 1" };
            var town2 = new Town { Id = Guid.NewGuid(), Name = "Town 2" };
            unitOfWork.Towns.Add(town1);
            unitOfWork.Towns.Add(town2);
            var route = new Route { Id = Guid.NewGuid(), StartingPoint = town1.Id, Destination = town2.Id };
            unitOfWork.Routes.Add(route);
            var game = new MonsterHuntGame(player, unitOfWork);
            Assert.Throws<DefeatedException>(() => game.GoToTown("town 2"));
        }
    }
}
