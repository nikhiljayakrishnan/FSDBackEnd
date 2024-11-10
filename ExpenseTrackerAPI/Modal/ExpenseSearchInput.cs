namespace ExpenseTrackerAPI.Modal
{
    public class ExpenseSearchInput
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public int UserId { get; set; }

    }
}
