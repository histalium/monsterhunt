using System;

namespace MonsterHunt.Commands
{
    internal class InventoryCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public InventoryCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string value)
        {
            var inventory = game.GetInventory();
            foreach (var item in inventory)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}