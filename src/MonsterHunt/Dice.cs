using System;

namespace MonsterHunt
{
    internal class Dice
    {
        private readonly Random random;

        public Dice()
        {
            random = new Random();
        }

        public int Roll()
        {
            return random.Next(6) + 1;
        }
    }
}