namespace ExpenseTrackerAPI.Modal
{
    public class EmailPhoneVerificationInput
    {
        public int UserId { get; set; }
        public string EmailCode { get; set; }
        public string PhoneNumberCode { get; set; }
        public VerificationType VerificationType { get; set; }

    }
    public enum VerificationType
    {
        PhoneNumber,
        Email
    }
}
