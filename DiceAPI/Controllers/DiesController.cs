using DiceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiesController : ControllerBase
    {
        /// <summary>
        /// Create, roll dice and return a list of every die thrown.
        /// </summary>
        /// <param name="qty">int: Number of dice to "throw" (1:1000)</param>
        /// <param name="sides">int: Number of sides on a die (1:1000)</param>
        /// <param name="adj">int: Adjustment to apply to the total result (MinInt:MaxInt)</param>
        /// <returns>List of Dies that will be serialized by JSON</returns>
        [HttpGet("details_qty/{qty}/sides/{sides}/adj/{adj}")]
        public ActionResult<List<Dies>> GetDetails(int qty, int sides, int adj)
        {
            List<Dies> Dice = new();    // List of Dies to hold return

            try
            {
                // Create and roll all the dice
                Dice Rolls = new(qty, sides, adj);

                for (int Idx = 0; Idx < Rolls.DiceCup.Count; Idx++)
                {
                    Dice.Add(Rolls.Details[Idx]);
                }
            }
            catch (Exception e)
            {
                return BadRequest("Dice rolling error: " + e.Message);
            }
            if (Dice.Count == 0)
            {
                return NotFound();
            }
            return Dice; // Ok(Dice);
        }

        /// <summary>
        /// Create, roll dice and return the result.
        /// </summary>
        /// <param name="qty">int: Number of dice to "throw" (1:1000)</param>
        /// <param name="sides">int: Number of sides on a die (1:1000)</param>
        /// <param name="adj">int: Adjustment to apply to the total result (MinInt:MaxInt)</param>
        /// <returns>ActionResult: List of Dies (one record in this case) will be serialized as JSON</returns>
        [HttpGet("total_qty/{qty}/sides/{sides}/adj/{adj}")]
        public ActionResult<Dies> GetDies(int qty, int sides, int adj)
        {
            Dies dies = new();          // Dice roller instance

            try
            {
                Dice Rolls = new(qty, sides, adj);      // Dice roller class
                int AResult = Rolls.Results();          // Return rolled results

                // Map results
                dies.Id = 0;
                dies.Qty = qty;
                dies.Sides = sides;
                dies.Result = AResult;
                dies.Total = AResult;
            }
            catch (Exception e)
            {
                return BadRequest("Dice rolling error: " + e.Message);
            }
            if (dies.Result == 0)
            {
                return NotFound();
            }
            return dies;  
        }

        /// <summary>
        /// Take a dice notation string to determine what dice we need to
        /// throw
        /// Issue: Http get won't pass a "+" to us without encoding it as "%2B". A side effect
        ///        of the encoding is to throw an exception... 
        ///        (IIS 10.0 Detailed Error - 404.11 - Not Found) and never finds this route. 
        ///        A possible fix is "modify the 
        ///        configuration/system.webServer/security/requestFiltering@allowDoubleEscaping 
        ///        setting.". This can make us vulnerable to malformed URLs though.
        /// </summary>
        /// <param name="cmd">string: Dice notation string (quantityDsides[[+|-}adjustment], 
        /// ie 1D6+1)</param>
        /// <returns>ActionResult: List of Dies (one record in this case)</returns>
        [HttpGet("dand/{cmd}")]
        public ActionResult<Dies> GetDandD(string cmd)
        {
            int Quantity = 0;           // Default 1D6
            int Sides = 6;
            int Adjustment = 0;
            Dies dies = new();          // Dice roller instance

            if (cmd.Contains("%2B"))
            {
                cmd = cmd.Replace("%2B", "+");
            }

            Parse(cmd, ref Quantity, ref Sides, ref Adjustment);

            try
            {
                Dice Rolls = new(Quantity, Sides, Adjustment);
                int AResult = Rolls.Results();          // Return rolled results

                // Map the results
                dies.Id = 0;
                dies.Qty = Quantity;
                dies.Sides = Sides;
                dies.Result = AResult;
                dies.Total = AResult;
            }
            catch (Exception e)
            {
                return BadRequest("Dice rolling error: " + e.Message);
            }
            if (dies.Result == 0)
            {
                return NotFound();
            }
            return dies; 
        }

        /// <summary>
        /// Parse the nDs[[+|-]n] dice notation string and return Quantity, Sides and
        /// Adjustment by reference to the caller
        /// </summary>
        /// <param name="arg">string: Dice notation to be parsed</param>
        /// <param name="Quantity">int; The number of dice to roll (1:1000)</param>
        /// <param name="Sides">int: The number of side a die has (0:1000) (0 allows for
        /// a special case "coin flip" mode that returns 0 or 1)</param>
        /// <param name="Adjustment">int: Adjustment that is applied to the total result
        /// of the dice throws</param>
        private static void Parse(string arg, ref int Quantity, ref int Sides, ref int Adjustment)
        {
            string arrg = arg.Trim().ToUpper();
            string[] parm1 = { "D" }; //Dies/sides delimiter
            string[] parm2 = { "+", "-" }; //Adjustment delimiters

            // Parse individual dice roll command or default to 1d6+0

            // No arguments, use 1D6 default
            if (arrg.Length == 0)
            {
                Quantity = 1;
                Sides = 6;
                Adjustment = 0;
                return;
            }

            // We started with a D (implies a singlr die)
            if (arrg.StartsWith("D"))
            {
                // Just a "D"? Default with 1D6+0
                if (arrg.Length == 1)
                {
                    Quantity = 1;
                    Sides = 6;
                    Adjustment = 0;
                    return;
                }
                //More to do here... default to one die and continue
                Quantity = 1;
                arrg = arrg[1..];
            }
            else
            {
                //Didn't start with a "D" split using "D" as a delimiter. The first argument
                //should be the die count
                string[] ary = arrg.Split(parm1, StringSplitOptions.RemoveEmptyEntries);
                string sside = ary[0];
                try
                {
                    Quantity = Convert.ToInt32(sside);
                }
                catch
                {
                    // Default to one die if dice count is bad
                    Quantity = 1;
                }
                if (ary.Length == 1)
                {
                    // Only die count, default to 6 sides die with no adjustment
                    Sides = 6;
                    Adjustment = 0;
                    return;
                }
                // More to do after "D", pass it along
                arrg = ary[1];
            }

            // Split the remaining parts using "+" or "-" as a delimiter
            string[] ary2 = arrg.Split(parm2, StringSplitOptions.RemoveEmptyEntries);
            if (ary2.Length == 1)
            {
                // We only got a single result which should be the number of sides
                // on the die with no adjustment
                try
                {
                    Sides = Convert.ToInt32(ary2[0]);
                }
                catch
                {
                    // Default to 6 sides if value is bogus
                    Sides = 6;
                }
                Adjustment = 0;
            }
            else
            {
                // More than a single result, should have die side count and adjustment
                if (ary2.Length > 1)
                {
                    try
                    {
                        Sides = Convert.ToInt32(ary2[0]);
                    }
                    catch
                    {
                        // Thisis so a bogus adjustment will default to a 6 sided die
                        Sides = 6;
                    }
                    try
                    {
                        Adjustment = Convert.ToInt32(ary2[1]);
                    }
                    catch
                    {
                        // This is so a bogus adjustment will default to 0 adjustment
                        Adjustment = 0;
                    }
                    if (Adjustment > 0)
                    {
                        if (arrg.Contains("-"))
                        {
                            // if the delimiter was a "-", negate the adjustment
                            Adjustment = 0 - Adjustment;
                        }
                    }
                }
                if (ary2.Length <= 0)
                {
                    // Nothing to see here, default six sided die and no adjustment
                    Sides = 6;
                    Adjustment = 0;
                }
            }
        }
    }
}
