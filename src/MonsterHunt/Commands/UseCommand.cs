using System;

namespace MonsterHunt.Commands
{
    internal class UseCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public UseCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string itemName)
        {
            try
            {
                game.UseItem(itemName);
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You do not have this item");
            }
            catch (CanNotUseItemException)
            {
                Console.WriteLine("Iten can't be used");
            }
        }
    }
}