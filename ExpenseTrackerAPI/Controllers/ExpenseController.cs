
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Data.Entities;
using ExpenseTrackerAPI.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ExpenseController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public ActionResult<ExpenseDto> Post([FromBody] ExpenseInput expenseInput)
        {
            var expense = new Expense()
            {
                Amount = expenseInput.Amount,
                Date = expenseInput.Date,
                Type = expenseInput.Type,
                UserId = expenseInput.UserId
            };
            _dbContext.Expenses.Add(expense);
            _dbContext.SaveChanges();
            var user = _dbContext.Users.FirstOrDefault(s => s.Id == expense.UserId);
            if (user != null)
            {
                user.Income = user.Income - expense.Amount;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
            }
            return new ExpenseDto()
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                Type = expense.Type,
                User = expense.User.FirstName + "" + expense.User.LastName,
                UserId = expense.UserId
            };
        }

        // GET: api/Expense
        [HttpGet("{id}")]
        public ActionResult<Expense> Get(int id)
        {
            return _dbContext.Expenses.FirstOrDefault(s => s.Id == id);
        }

        [HttpGet("ExpenseSearch")]
        public ActionResult<IEnumerable<ExpenseDto>> ExpenseSearch([FromQuery] ExpenseSearchInput searchInput)
        {
          //  DateTime indianTime = DateTime.Parse("2024-11-10"); 
            var fromDate = DateTime.Parse(searchInput.FromDate.ToString("dd/MM/yyyy"));
            var toDate = DateTime.Parse(searchInput.ToDate.ToString("dd/MM/yyyy"));
           
            var expenses = _dbContext.Expenses.Include(s=>s.User)
                .Where(s => s.Date >= fromDate && s.Date <= toDate && s.UserId==searchInput.UserId)
                .Select(s=>new ExpenseDto()
                {
                    Id = s.Id,
                    Amount = s.Amount,
                    Date = s.Date,
                    Type = s.Type,
                    User = s.User.FirstName +"" + s.User.LastName,
                    UserId = s.UserId
                })
                .ToList();

            return expenses;
        }

    }
}
