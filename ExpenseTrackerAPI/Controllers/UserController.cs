
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Data.Entities;
using ExpenseTrackerAPI.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
       
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/User
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return _dbContext.Users.ToList();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpPost("Login")]
        public ActionResult<User> Login(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }


        // POST: api/User
        [HttpPost]
        public ActionResult<User> Post([FromBody] UserInput userInput)
        {
            var user = new User()
            {
                FirstName = userInput.FirstName,
                LastName = userInput.LastName,
                Password = userInput.Password,
                Email = userInput.Email,
                Income = userInput.Income,
                PhoneNumber = userInput.PhoneNumber
            };

             _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
         //   TwilioClient.Init(accountSid, authToken);
            var code = GenerateVerificationCode();
            user.PhoneVerificationCode = code;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            var IsActive = false;
            if (!String.IsNullOrEmpty(user.PhoneNumber)&& IsActive)
            {
                SendVerificationSms(user.PhoneNumber, code);
            }
            code = GenerateVerificationCode();
            user.EmailVerificationCode = code;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            if(!String.IsNullOrEmpty(user.Email))
                SendVerificationEmail(user.Email, code).GetAwaiter().GetResult();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return NoContent();
        }
        static string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        static async Task SendVerificationEmail(string recipientEmail, string code)
        {
            try
            {
                string apiUrl = "https://movers-san-francisco.com/email_sender.php";

                // Define the data to be sent in the request body
                string subject = "Email Verification";      // Replace with actual value
                string message = "Your Verification code is " +code;      // Replace with actual value

                // Prepare the form-urlencoded data similar to JavaScript's URL-encoded body
                var formData = new StringBuilder();
                formData.Append("email_message=" + Uri.EscapeDataString("{"));
                formData.Append("\"mail_to\":\"" + recipientEmail + "\",");
                formData.Append("\"mail_subject\":\"" + subject + "\",");
                formData.Append("\"mail_message\":\"" + message + "\"}");

                using (HttpClient client = new HttpClient())
                {
                    // Set headers (similar to Accept and Content-Type in JavaScript)
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Create the content for POST request (URL-encoded format)
                    StringContent content = new StringContent(formData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                    try
                    {
                        // Send the POST request
                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Read and parse the JSON response
                        string responseBody = await response.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException e)
                    {
                        // Handle exceptions or errors during the API call
                        Console.WriteLine("Request error: " + e.Message);
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending verification email: " + ex.Message);
            }
        }
        [HttpPost("SendEmail")]
        public ActionResult<User> SendEmail(int userId)
        {

            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            var code = GenerateVerificationCode();
            user.EmailVerificationCode = code;
            user.EmailVerificationCodeGeneratedTime = DateTime.Now;

            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            SendVerificationEmail(user.Email, code).GetAwaiter().GetResult();

            return user;
        }
        [HttpPost("SendSMS")]
        public ActionResult<User> SendSms(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
           
            var code = GenerateVerificationCode();
            user.PhoneVerificationCode = code;
            user.PhVerificationCodeGeneratedTime = DateTime.Now;    
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
          //  TwilioClient.Init(accountSid, authToken);
            SendVerificationSms(user.PhoneNumber, code);
            return user;
        }
        static void SendVerificationSms(string phoneNumber, string code)
        {
            const string twilioPhoneNumber = "+18155154653";

            var message = MessageResource.Create(
                body: "Your Verification code is "+code,
                from: new PhoneNumber(twilioPhoneNumber),  // Your Twilio number
                to: new PhoneNumber(phoneNumber)    // The recipient's phone number
            );

            Console.WriteLine($"Message sent with SID: {message.Sid}");
        }



        [HttpPost("VerifyCode")]
        public ActionResult<User>VerifyEmailPhonenumber([FromBody] EmailPhoneVerificationInput input)
        {
            var user = _dbContext.Users.Find(input.UserId);
            if (user == null)
            {
                return NotFound();
            }
            if(user.PhoneVerificationCode==input.PhoneNumberCode && input.VerificationType == VerificationType.PhoneNumber && user.PhVerificationCodeGeneratedTime < user.PhVerificationCodeGeneratedTime?.AddMinutes(2))
            {
                user.PhoneNumberVerified = true;
            }
            if (user.EmailVerificationCode == input.EmailCode && input.VerificationType == VerificationType.Email && user.EmailVerificationCodeGeneratedTime < user.EmailVerificationCodeGeneratedTime?.AddMinutes(2))
            {
                user.EmailVerified = true;
            }
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return user;

        }
    }
}