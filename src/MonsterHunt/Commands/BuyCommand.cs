using System;

namespace MonsterHunt.Commands
{
    internal class BuyCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public BuyCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string itemName)
        {
            try
            {
                game.BuyItem(itemName);
                Console.WriteLine($"Bought item. You have now {game.Player.Coins}c");
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
            }
            catch (NotAtAMerchantException)
            {
                Console.WriteLine("Not at a merchant");
            }
            catch (MerchantDoesNotOfferException)
            {
                Console.WriteLine("Merchant does not offer item");
            }
            catch (NotEnoughCoinsException)
            {
                Console.WriteLine("You don't have enough coins");
            }
        }
    }
}