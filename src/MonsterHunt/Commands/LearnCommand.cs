using System;

namespace MonsterHunt.Commands
{
    internal class LearnCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public LearnCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string recipe)
        {
            try
            {
                game.Learn(recipe);
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You don't have this item");
            }
            catch (ItemNotRecipeException)
            {
                Console.WriteLine("Can't learn item");
            }
        }
    }
}