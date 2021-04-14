namespace DiceAPI.Models
{
    /// <summary>
    /// Record layout for result return record
    /// </summary>
    public class Dies
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public int Sides { get; set; }
        public int Adjustment { get; set; }
        public int Result { get; set; }
        public int Total { get; set; }
    }
}
