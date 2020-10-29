using System;

namespace MonsterHunt.Commands
{
    internal class AttackCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public AttackCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string value)
        {
            try
            {
                game.Attack();
            }
            catch (NotInBattleModeException)
            {
                Console.WriteLine("You are not in a battle");
            }
        }
    }
}