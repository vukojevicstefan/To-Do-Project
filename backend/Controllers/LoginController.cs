using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using To_Do_Project.DTOs;
using To_Do_Project.Models;

namespace Controllers;


[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    public Context _context;
    private IConfiguration _configuration;

    public LoginController(Context context, IConfiguration config)
    {
        _context = context;
        _configuration = config;
    }


    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginUserDTO dto)
    {
        string Email = dto.Email;
        string Password = dto.Password;

        if (string.IsNullOrWhiteSpace(Email))
            return BadRequest("You have to enter your email.");
        if (string.IsNullOrWhiteSpace(Password))
            return BadRequest("You have to enter a password.");
         if (_configuration == null)
            return BadRequest("Configuration is null, please check appsettings.json");
         if (_configuration["Jwt:Key"] == null)
            return BadRequest("Key is null, please check appsettings.json");
         if (_configuration["Jwt:Issuer"] == null)
            return BadRequest("Issuer is null, please check appsettings.json");
         if (_configuration["Jwt:Audience"] == null)
            return BadRequest("Audience is null, please check appsettings.json");
        if (_context == null)
            return BadRequest("Database context is null.");
        if (_context.Users == null)
            return BadRequest("Users table is null in the database.");
        if (!await _context.Users.AnyAsync(u => u.Email.ToLower() == Email.ToLower()))
            return Unauthorized("Email not found");

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == Email.ToLower());
            if (user == null)
                return Unauthorized("Email not found");
            if (VerifyPassword(Password, user.Password, user.Salt))
                    return Ok(new { token = Generate(user) });
                else
                    return Unauthorized("Wrong password");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or white space", "password");
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash", "passwordHash");
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt", "passwordHash");

        using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != storedHash[i]) return false;
        }

        return true;
    }
    private object Generate(User u)
    {
        var s= _configuration["Jwt:Key"];
        if(s == null)
            return BadRequest("Key is null, please check appsettings.json");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s));
        if (securityKey == null)
            return BadRequest("Security key is null, please check appsettings.json");
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier,u.Username),
            new Claim(ClaimTypes.Email,u.Email),
            new Claim(ClaimTypes.Sid,u.Id.ToString())
        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [AllowAnonymous]
    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUpUser([FromBody] RegisterUserDTO dto)
    {
        string Username = dto.Username;
        string Email = dto.Email;
        string Password = dto.Password;
        string ConfirmPassword = dto.ConfirmPassword;
        try
        {
            if (string.IsNullOrWhiteSpace(Username) || Username.Length > 20)
                return BadRequest("You have to enter a name shorter than 20 characters.");
            if (string.IsNullOrWhiteSpace(Email))
                return BadRequest("You have to enter email.");
            if (string.IsNullOrWhiteSpace(Password))
                return BadRequest("You have to enter a password.");
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                return BadRequest("You have to enter a password again.");
            if (ConfirmPassword != Password)
                return BadRequest("Passwords do not match.");
            if (await _context.Users.AnyAsync(u => u.Username == Username))
                return BadRequest("Username is already taken.");
            if (await _context.Users.AnyAsync(u => u.Email == Email))
                return BadRequest("Email already registered.");

            User user = new User
            {
                Username = Username,
                Email = Email
            };

            byte[] passwordHash, passwordSalt;
            try{
                CreatePasswordHash(Password, out passwordHash, out passwordSalt);
            }
            catch(Exception e){ 
                return BadRequest(e.Message);
            }
            user.Password = passwordHash;
            user.Salt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var verify = await _context.Users.Where(p => p.Email == Email).FirstOrDefaultAsync();
            if (verify == null)
                return BadRequest("Error with creating user.");
            try
            {
                await Verification(verify);
            }
            catch (Exception)
            {
                return BadRequest("Email address is not valid.");
            }

            return Ok("User created successfully.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or white space.");

        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private async Task Verification(User user)
    {
        var emailConfig = _configuration.GetSection("EmailSettings");

        string smtpUser = emailConfig["SmtpUser"] ?? throw new Exception("SMTP user is missing in config.");
        string smtpPass = emailConfig["SmtpPass"] ?? throw new Exception("SMTP password is missing in config.");
        string fromName = emailConfig["FromName"] ?? "NoName";
        string fromEmail = emailConfig["FromEmail"] ?? throw new Exception("FromEmail is missing in config.");
        
        if (!int.TryParse(emailConfig["SmtpPort"], out int port))
            throw new Exception("SMTP port is not set or invalid.");

        string host = emailConfig["SmtpHost"] ?? throw new Exception("SMTP host is missing in config.");
        string toEmail = user.Email ?? throw new Exception("User email is null.");

        string message = $"Dear {user.Username}, Welcome to the To-Do Lists Project community.\n\nWith respect,\nStefan Vukojevic.";

        using SmtpClient client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUser, smtpPass)
        };

        MailAddress fromMail = new MailAddress(fromEmail, fromName);
        MailAddress toMail = new MailAddress(toEmail, user.Username);

        using MailMessage mesg = new MailMessage()
        {
            From = fromMail,
            Subject = "Welcome to To-Do Lists Project",
            Body = message
        };

        mesg.To.Add(toMail);
        await client.SendMailAsync(mesg);
    }
}