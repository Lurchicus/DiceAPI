using System;

namespace DiceAPI
{
    class Die
    {
        private readonly Random Chance = new();

        private int _Id;
        private int _Sides;
        private int _Result;

        public Die()
        {
            Sides = 6;
            Id = 0;
            Result = Toss();
        }

        public Die(int DieId, int DieSides)
        {
            if (DieSides >= 0 && DieSides <= 1000)
            {
                Sides = DieSides;
            }
            else
            {
                throw new Exception("Die error: "+DieSides.ToString()+" sides is out of range (0:1000).");
            }
            Id = DieId;
            Result = Toss();
        }

        private int Toss()
        {
            if (Sides == 1)
            {
                // Two sided die (0 or 1) (coin toss)
                Result = Chance.Next(0, Sides + 1);
            }
            else
            {
                // Up to 1000 sides, otherwise don't overflow an int
                Result = Chance.Next(1, Sides + 1);
            }
            return Result;
        }

        public int GetResult()
        {
            return Result;
        }

        public int Id
        {
            get => _Id;
            set => _Id = value;
        }

        public int Sides
        {
            get => _Sides;
            set => _Sides = value;
        }

        public int Result
        {
            get => _Result;
            private set => _Result = value;
        }
    }
}
