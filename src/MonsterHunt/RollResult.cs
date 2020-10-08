using System;

namespace MonsterHunt
{
    internal class RollResult<T>
    {
        public T Roll1 { get; set; }
        public T Roll2 { get; set; }
        public T Roll3 { get; set; }
        public T Roll4 { get; set; }
        public T Roll5 { get; set; }
        public T Roll6 { get; set; }

        public T GetResult(int roll)
        {
            switch (roll)
            {
                case 1:
                    return Roll1;

                case 2:
                    return Roll2;

                case 3:
                    return Roll3;

                case 4:
                    return Roll4;

                case 5:
                    return Roll5;

                case 6:
                    return Roll6;

                default:
                    throw new Exception("invalid roll");
            }
        }

        public RollResult<T> Set(int roll, T value)
        {
            switch (roll)
            {
                case 1:
                    Roll1 = value;
                    break;

                case 2:
                    Roll2 = value;
                    break;

                case 3:
                    Roll3 = value;
                    break;

                case 4:
                    Roll4 = value;
                    break;

                case 5:
                    Roll5 = value;
                    break;

                case 6:
                    Roll6 = value;
                    break;

                default:
                    throw new Exception("invalid roll");
            }

            return this;
        }

        public RollResult<T> Set(int rollMin, int rollMax, T value)
        {
            for (var i = rollMin; i <= rollMax; i++)
            {
                Set(i, value);
            }

            return this;
        }
    }
}