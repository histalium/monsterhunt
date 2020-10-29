using System;

namespace MonsterHunt.Commands
{
    internal class OffersCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public OffersCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string value)
        {
            try
            {
                var requests = game.GetOffers();
                foreach (var (item, price) in requests)
                {
                    Console.WriteLine($"{item.Name} ({price}c)");
                }
            }
            catch (NotAtAMerchantException)
            {
                Console.WriteLine("Not at a merchant");
            }
        }
    }
}