using System;

namespace MonsterHunt.Commands
{
    internal class MakeCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public MakeCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string recipeName)
        {
            try
            {
                game.Make(recipeName);
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You don't have this item");
            }
            catch (DoNotKnowRecipeException)
            {
                Console.WriteLine("You don't know this recipe");
            }
            catch (MissingIngredientsException)
            {
                Console.WriteLine("You have missing ingredients");
            }
        }
    }
}