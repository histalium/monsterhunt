using System;
using System.Linq;

namespace MonsterHunt.Commands
{
    internal class RecipesCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public RecipesCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string value)
        {
            var recipes = game.GetRecipes();
            if (recipes.Any())
            {
                foreach (var recipe in recipes)
                {
                    Console.WriteLine(recipe.Name);
                }
            }
            else
            {
                Console.WriteLine("No recipes");
            }
        }
    }
}