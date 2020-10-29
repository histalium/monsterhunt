using System;

namespace MonsterHunt.Commands
{
    internal class StatsCommand : ICommand
    {
        private readonly MonsterHuntGame game;

        public StatsCommand(MonsterHuntGame game)
        {
            this.game = game;
        }

        public void Execute(string value)
        {
            var stats = game.GetStats();

            Console.WriteLine($"Health:     {stats.Health}/{stats.MaxHealth}");
            Console.WriteLine($"Attack:     {stats.Attack}");
            Console.WriteLine($"Defence:    {stats.Defence}");
            Console.WriteLine();
            if (string.IsNullOrEmpty(stats.WeaponName))
            {
                Console.WriteLine($"Weapon:");
            }
            else
            {
                Console.WriteLine($"Weapon:     {stats.WeaponName} (Attack +{stats.WeaponAttack})");
            }
            if (string.IsNullOrEmpty(stats.BodyArmorName))
            {
                Console.WriteLine($"Body armor:");
            }
            else
            {
                Console.WriteLine($"Body armor: {stats.BodyArmorName} (Defence +{stats.BodyArmorDefence})");
            }
            if (string.IsNullOrEmpty(stats.LegArmorName))
            {
                Console.WriteLine($"Leg armor:");
            }
            else
            {
                Console.WriteLine($"Leg armor:  {stats.LegArmorName} (Defence +{stats.LegArmorDefence})");
            }
        }
    }
}