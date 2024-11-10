namespace ExpenseTrackerAPI.Modal
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string User { get; set; } 
    }
}
