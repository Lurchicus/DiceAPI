using System;

namespace DiceAPI
{
    /// <summary>
    /// Create and throw a die
    /// </summary>
    class Die
    {
        // Pseudo random number generator
        private readonly Random Chance = new();

        private int _Id;        // Optional number for a die
        private int _Sides;     // 1 to 1000 sides for a die
        private int _Result;    // Throw result

        /// <summary>
        /// Default constructor
        /// a single die with 6 sides
        /// Note: Die is thrown as soon as it is created
        /// </summary>
        public Die()
        {
            Sides = 6;
            Id = 0;
            Result = Toss();
        }

        /// <summary>
        /// Constructor overload
        /// Create a die with 1 to 1000 sides 
        /// Note: Die is thrown as soon as it is created
        /// </summary>
        /// <param name="DieId">int: Id for die (will be 0 or some positive number</param>
        /// <param name="DieSides">int: 1 to 1000 sides</param>
        public Die(int DieId, int DieSides)
        {
            if (DieSides >= 1 && DieSides <= 1000)
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

        /// <summary>
        /// Toss the die and return the result
        /// </summary>
        /// <returns>int: Result of die throw</returns>
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

        /// <summary>
        /// Return the die throw results method
        /// </summary>
        /// <returns></returns>
        public int GetResult()
        {
            return Result;
        }

        /// <summary>
        /// Property: Die ID (0 or 1 to maxint)
        /// </summary>
        public int Id
        {
            get => _Id;
            set => _Id = value;
        }

        /// <summary>
        /// Property: Die Sides (1 to 1000 sides)
        /// </summary>
        public int Sides
        {
            get => _Sides;
            set => _Sides = value;
        }

        /// <summary>
        /// Property: Resullt of throwing this die
        /// Note: The result can't be altered once thrown
        /// </summary>
        public int Result
        {
            get => _Result;
            private set => _Result = value;
        }
    }
}
