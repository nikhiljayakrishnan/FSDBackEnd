namespace ExpenseTrackerAPI.Modal
{
    public class ExpenseInput
    {
        public double Amount { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}
