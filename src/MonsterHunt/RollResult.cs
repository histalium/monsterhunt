using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class RollResult
    {
        private readonly Dictionary<int, Guid?> results;

        public RollResult()
        {
            results = Enumerable.Range(1, 6)
                .ToDictionary(t => t, t => (Guid?)null);
        }

        public Guid? GetResult(int roll)
        {
            ValidateRoll(roll);

            return results[roll];
        }

        public RollResult Set(int roll, Guid? value)
        {
            ValidateRoll(roll);

            results[roll] = value;

            return this;
        }

        public RollResult Set(int rollMin, int rollMax, Guid? value)
        {
            for (var i = rollMin; i <= rollMax; i++)
            {
                Set(i, value);
            }

            return this;
        }

        private static void ValidateRoll(int roll)
        {
            if (roll < 1 || roll > 6)
            {
                throw new Exception("invalid roll");
            }
        }
    }
}