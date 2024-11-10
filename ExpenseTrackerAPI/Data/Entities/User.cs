using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ExpenseTrackerAPI.Data.Enum;

namespace ExpenseTrackerAPI.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double Income { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public UserType Type { get; set; }
        public User()
        {
            Type = UserType.Free;
        }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberVerified { get; set; }
        public bool EmailVerified { get; set; }
        public string? PhoneVerificationCode { get; set; }
        public DateTime? PhVerificationCodeGeneratedTime { get; set; }
        public string? EmailVerificationCode { get; set;}
        public DateTime? EmailVerificationCodeGeneratedTime { get; set; }

    }
}
