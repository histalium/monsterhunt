using System;

namespace MonsterHunt.Commands
{
    internal class GoToCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public GoToCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string destination)
        {
            try
            {
                game.GoToTown(destination);
                return;
            }
            catch (InvalidTownException)
            {
                // do nothing. try other command.
            }
            catch (SameTownException)
            {
                Console.WriteLine("Same town as current");
                return;
            }
            catch (NoRouteToTownException)
            {
                Console.WriteLine("No route to town");
            }

            try
            {
                game.GoToMerchant(destination);
                Console.WriteLine($"Welcome to {game.CurrentMerchant.Name}");
                return;
            }
            catch (InvalidMerchantException)
            {
                // do nothing. try other command.
            }
            catch (SameMerchantException)
            {
                Console.WriteLine("Same merchant as current");
                return;
            }
            catch (MerchantNotInTownException)
            {
                // do nothing. try other command.
            }

            Console.WriteLine("Invalid location");
        }
    }
}