using DiceAPI.Models;
using System;
using System.Collections.Generic;

namespace DiceAPI
{
    /// <summary>
    /// Create and throw multiple dice using die class
    /// </summary>
    class Dice
    {
        public List<Die> DiceCup = new();   // A nice list object to hold our dies
        public List<Dies> Details = new();

        private int _Quantity;      // 1 to 1000 dies
        private int _Sides;         // 1 to 1000 sides (1 side is for a 1 or 0 coin toss)
        private int _Adjustment;    // Value to add to the sum of our results

        /// <summary>
        /// Default constructor throws a singler 6 sides die
        /// Note: Dice are created and destroyed for each API call
        /// </summary>
        public Dice()
        {
            // Default to 1D6+0
            Quantity = 1;
            Sides = 6;
            Adjustment = 0;
            Die ADie = new(0, Sides);
            DiceCup.Add(ADie);
        }

        /// <summary>
        /// Constructor overload
        /// Rolls 1 to 1000 dies (dice) with 0 to 1000 sides with an sdjustment to the 
        /// total result.
        /// Note: The limit of 1000 for dies and sides is totally arbitrary and could be
        ///       fine tuned higher. It's there since the program instansiates a die 
        ///       class for each die requested so we need to avoid using too much 
        ///       memory. 1000 is VERY conservitive by the way.
        /// Note: Dice are created and destroyed for each API call
        /// </summary>
        /// <param name="DiceQuantity">int: 1 to 1000 dice</param>
        /// <param name="DiceSides">int: 1 to 1000 sides</param>
        /// <param name="DiceAdjustment">int: Adjustment to total result</param>
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
            if (DiceSides >= 1 && DiceSides <= 1000)
            {
                Sides = DiceSides;
            }
            else
            {
                throw new Exception("Dice error: " + DiceSides.ToString() + " die sides is out of range (1:1000).");
            }
            Adjustment = DiceAdjustment;

            // Create a die for each throw
            for (int DiceId = 0; DiceId < Quantity; DiceId++)
            {
                try
                {
                    Die ADie = new(DiceId, DiceSides);  // Create and toss a die
                    DiceCup.Add(ADie);                  // Save it in a "dice cup"
                    Dies ADetail = new();               // Create a "Dice" detail object
                    ADetail.Id = ADie.Id;               // Die ID
                    ADetail.Qty = DiceQuantity;         // Quantity (same for all)
                    ADetail.Sides = ADie.Sides;         // Sides (also the same for all)
                    ADetail.Adjustment = DiceAdjustment;    // Adjustment (same)
                    ADetail.Result = ADie.Result;       // Get the roll result for the die
                    ADetail.Total = Results();          // By putting this here, I get a running adjusted total
                    Details.Add(ADetail);               // Shove the details into a list (1:1 with Dice Cup)
                }
                catch (OutOfMemoryException e)
                {
                    DiceCup.Clear();
                    Details.Clear();
                    throw new OutOfMemoryException("Out of memory exception error: "+e.Message, e);
                }
                catch (Exception e)
                {
                    DiceCup.Clear();
                    Details.Clear();
                    throw new Exception("Dice exception, creating die.", e);
                }
            }
        }

        /// <summary>
        /// Get and return the total throw results and adjust
        /// </summary>
        /// <returns>int: Adjusted sum of all throws</returns>
        public int Results()
        {
            int Sum = 0;
            for (int Id = 0; Id < DiceCup.Count; Id++)
            {
                Sum += DiceCup[Id].Result;
            }
            return Sum + Adjustment;
        }

        /// <summary>
        /// Dice Quantity (1:1000)
        /// </summary>
        public int Quantity
        {
            get => _Quantity;
            set => _Quantity = value;
        }

        /// <summary>
        /// Dice Sides (1:1000)
        /// </summary>
        public int Sides
        {
            get => _Sides;
            set => _Sides = value;
        }

        /// <summary>
        /// Dice Adjustment (MinInt:MaxInt)
        /// </summary>
        public int Adjustment
        {
            get => _Adjustment;
            set => _Adjustment = value;
        }
    }
}
