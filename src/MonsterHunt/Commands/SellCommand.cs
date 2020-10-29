using System;

namespace MonsterHunt.Commands
{
    internal class SellCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public SellCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string itemName)
        {
            try
            {
                game.SellItem(itemName);
                Console.WriteLine($"Item sold. You have now {game.Player.Coins}c");
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
            }
            catch (NotAtAMerchantException)
            {
                Console.WriteLine("Not at a merchant");
            }
            catch (MerchantDoesNotRequestException)
            {
                Console.WriteLine("Merchant does not request item");
            }
            catch (NotEnoughCoinsException)
            {
                Console.WriteLine("You don't have enough coins");
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You do not have this item");
            }
        }
    }
}