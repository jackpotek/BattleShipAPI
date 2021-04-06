using System;

namespace Battleships.Engine
{
    public class RandomGenerator
    {
        private readonly Random random;
        public RandomGenerator(Random random = default(Random))
        {
            this.random = random ?? new Random();
        }

        public int GetRandomNumber(int max)
        {
            int randomInt = random.Next(1, max);
            return randomInt;
        }

    }
}
