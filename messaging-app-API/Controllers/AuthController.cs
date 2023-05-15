
using messaging_app_API.Data;
using messaging_app_API.Dto;
using messaging_app_API.Helper;
using messaging_app_API.Hubs;
using messaging_app_API.Models;
using messaging_app_API.Services;
using messaging_app_API.UtilityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace LegalGen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Dbcontext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ChatService _chatService;
        private readonly IHubContext<MessageHub> _hubContext;

        public AuthController(Dbcontext dbcontext, IConfiguration configuration, IEmailService emailService,
            ChatService chatService, IHubContext<MessageHub> hubContext)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _emailService = emailService;
            _chatService = chatService;
            _hubContext = hubContext;
        }

        //getUser 
        [HttpGet("getAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _dbcontext.Users.ToListAsync();
            return Ok(users);
        }

        //Api user Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                // Return a response indicating that the email is already registered
                return BadRequest("Email is already registered.");
            }

            var user = new User
            {
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                Name = request.Name,

            };
            // Save the user to the database
            _chatService.AddUserToList(request.Name);
            await _dbcontext.Users.AddAsync(user);
            await _dbcontext.SaveChangesAsync();

            return Ok("Registration Successfull");
        }

        //EventLogInformation into database
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserloginDto request)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            };
            if (!PasswordHasher.Verifypassword(request.Password, user.Password))
            {
                return BadRequest("Invalid password");
            };
            string token = CreateToken(user);
            user.Token = token;
            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                id = user.Id,
                user.Token,
                Message = "Login SucessFully"
            });
        }

        [HttpPost("sendMessage")]
        //[Authorize]
        public async Task<IActionResult> SendMessage(Message message)
        {
            // Set message timestamp and user
            message.Timestamp = DateTime.UtcNow;
            //message.User = User.Identity.Name;

            // Add message to database
            _dbcontext.Messages.Add(message);
            await _dbcontext.SaveChangesAsync();

            // Send message to all clients
            await _hubContext.Clients.All.SendAsync("ReceiveMessage",message.message, message.userId);

            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                message
            });
        }

        [HttpGet("ReceiveMessage")]
        //[Authorize]
        public async Task<IEnumerable<Message>> GetMessages()
        {
            // Retrieve all messages from database
            return await _dbcontext.Messages.ToListAsync();
        }

       
        //create token 
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        //verifyToken 



        //reset  password and email 
        //[HttpPost("send-reset-email/{email}")]
        //public async Task<IActionResult> SendEmail(string email)
        //{
        //    var user = await _dbcontext.Users.FirstOrDefaultAsync(a => a.Email == email);
        //    if (user is null)
        //    {
        //        return NotFound(new
        //        {
        //            StatusCode = 404,
        //            Message = "Email Dosen't Exist"
        //        });
        //    }
        //    var tokenBytes = RandomNumberGenerator.GetBytes(64);
        //    Console.WriteLine(tokenBytes);
        //    var emailToken = Convert.ToBase64String(tokenBytes);
        //    user.ResetPasswordToken = emailToken;
        //    user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
        //    string from = _configuration["EmailSettings:From"];
        //    var emailModel = new EmailModel(email, "Reset Password!", EmailBody.EmailStringBody(email, emailToken));
        //    _emailService.SendEmail(emailModel);
        //    _dbcontext.Entry(user).State = EntityState.Modified;
        //    await _dbcontext.SaveChangesAsync();
        //    return Ok(new
        //    {
        //        email = user.Email,
        //        resetToken = user.ResetPasswordToken,
        //        StatusCode = 200,
        //        Message = "Email Sent!"
        //    });
        //}

        //resetPassword 
        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDto resetpassword)
        //{
        //    var newToken = resetpassword.EmailToken.Replace(" ", "+");
        //    var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Email == resetpassword.Email);
        //    if (user == null) { return NotFound("User Not Found"); }
        //    var tokenCode = user.ResetPasswordToken;
        //    DateTime emailTokenExpiry = user.ResetPasswordExpiry;
        //    if (tokenCode != resetpassword.EmailToken || emailTokenExpiry < DateTime.Now)
        //    {
        //        return BadRequest("Invalid reset link");
        //    };
        //    user.Password = PasswordHasher.HashPassword(resetpassword.NewPassword);
        //    _dbcontext.Entry(user).State = EntityState.Modified;
        //    await _dbcontext.SaveChangesAsync();
        //    return Ok("Password reset sucessfully");
        //}

    }
}
