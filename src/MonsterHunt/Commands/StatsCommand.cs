using System;
using System.Linq;

namespace MonsterHunt.Commands
{
    internal class StatsCommand : ICommand
    {
        private readonly MonsterHuntGame game;
        private readonly UnitOfWork unitOfWork;

        public StatsCommand(MonsterHuntGame game, UnitOfWork unitOfWork)
        {
            this.game = game;
            this.unitOfWork = unitOfWork;
        }

        public void Execute(string value)
        {
            var stats = game.GetStats();
            var elementAttackText = stats.ElementAttacks
                .Select(t => $"{unitOfWork.Elements.Get(t.Element).Name} +{t.Attack}")
                .ToList();
            var elementAttackTextWithBrackets = elementAttackText.Count() == 0 ? string.Empty : $"({string.Join(", ", elementAttackText)})";
            Console.WriteLine($"Health:     {stats.Health}/{stats.MaxHealth}");
            Console.WriteLine($"Attack:     {stats.Attack} {elementAttackTextWithBrackets}");
            Console.WriteLine($"Defence:    {stats.Defence}");
            Console.WriteLine();
            if (string.IsNullOrEmpty(stats.WeaponName))
            {
                Console.WriteLine($"Weapon:");
            }
            else
            {
                var weaponAttackText = string.Join(", ", new[] { $"Attack +{stats.WeaponAttack}" }
                    .Concat(elementAttackText));
                Console.WriteLine($"Weapon:     {stats.WeaponName} ({weaponAttackText})");
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