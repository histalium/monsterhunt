using System;

namespace MonsterHunt.Commands
{
    internal class EquipCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public EquipCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string itemName)
        {
            try
            {
                game.EquipWeapon(itemName);
                return;
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
                return;
            }
            catch (ItemNotAWeaponException)
            {
                // do nothing. try other command.
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You don't have this item");
                return;
            }

            try
            {
                game.EquipBodyArmor(itemName);
                return;
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
                return;
            }
            catch (ItemNotBodyArmorException)
            {
                // do nothing. try other command.
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You don't have this item");
                return;
            }

            try
            {
                game.EquipLegArmor(itemName);
                return;
            }
            catch (InvalidItemException)
            {
                Console.WriteLine("Invalid item");
                return;
            }
            catch (ItemNotLegArmorException)
            {
                // do nothing. try other command.
            }
            catch (DoNotOwnItemException)
            {
                Console.WriteLine("You don't have this item");
                return;
            }

            Console.WriteLine("Can't equip item");
        }
    }
}