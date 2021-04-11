using System;
using System.Collections.Generic;

namespace DiceAPI
{
    class Dice
    {
        public List<Die> DiceCup = new();   // A nice list object to hold our dies

        private int _Quantity;      // 1 to 1000 dies
        private int _Sides;         // 0 to 1000 sides (0 side is for a 1 or 0 coin toss)
        private int _Adjustment;    // Value to add to the sum of our results

        public Dice()
        {
            // Default to 1D6+0
            Quantity = 1;
            Sides = 6;
            Adjustment = 0;
            //List<Die> DiceCup = new() { };
            Die ADie = new(0, Sides);
            DiceCup.Add(ADie);
        }

        public Dice(int DiceQuantity, int DiceSides, int DiceAdjustment)
        {
            // Up to 1000 dies at a time
            if (DiceQuantity >= 1 && DiceQuantity <= 1000)
            {
                Quantity = DiceQuantity;
            }
            else
            {
                throw new Exception("Dice error: " + DiceQuantity.ToString() + " dies is out of range (1:1000).");
            }
            // Up to 1000 sides on a die
            if (DiceSides >= 0 && DiceSides <= 1000)
            {
                Sides = DiceSides;
            }
            else
            {
                throw new Exception("Dice error: " + DiceSides.ToString() + " die sides is out of range (0:1000).");
            }
            Adjustment = DiceAdjustment;
            //List<Die> DiceCup = new() { };

            // Create a die for each throw
            for (int DiceId = 0; DiceId < Quantity; DiceId++)
            {
                try
                {
                    Die ADie = new(DiceId, DiceSides);
                    DiceCup.Add(ADie);
                }
                catch (OutOfMemoryException e)
                {
                    DiceCup.Clear();
                    throw new OutOfMemoryException(e.Message, e);
                }
                catch (Exception e)
                {
                    DiceCup.Clear();
                    throw new Exception("Dice exception, creating die.", e);
                }
            }
        }

        public int Results()
        {
            int Sum = 0;
            for (int Id = 0; Id < DiceCup.Count; Id++)
            {
                Sum += DiceCup[Id].Result;
            }
            return Sum + Adjustment;
        }

        public int Quantity
        {
            get => _Quantity;
            set => _Quantity = value;
        }

        public int Sides
        {
            get => _Sides;
            set => _Sides = value;
        }

        public int Adjustment
        {
            get => _Adjustment;
            set => _Adjustment = value;
        }
    }
}
