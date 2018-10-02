using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using API_Til_IOT.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Til_IOT.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class JWTTokenController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly LoginDBContext _ldbContext;

        public JWTTokenController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            LoginDBContext ldbContext) {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _ldbContext = ldbContext;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {

            //LoginDBContext ldbc = HttpContext.RequestServices.GetService(typeof(LoginDBContext)) as LoginDBContext;
            
            return new string[] { "token1", "token2" };
        }

        [HttpPost]
        [Route("Login")]
        public async Task<object> Login([FromBody] LoginDTO model)
        {
            _ldbContext.Database.EnsureCreated();
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.email, model.password, false, false); //<-- sign in options, e.g. lockout on failure.

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.email);
                return await GenerateJwtToken(model.email, appUser);
            }

            string error = "USER_REGISTRATION_ERROR_";
            return error;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<object> Register([FromBody] RegisterDTO model)
        {
            if (!model.apikey.Equals(_configuration["API:APIKey"]))
            {
                return "Invalid API Key";
            }
            _ldbContext.Database.EnsureCreated();
            var user = new IdentityUser //<-- IdentityUserOptions
            {
                UserName = model.email,
                Email = model.email
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.email);
                await AddToRole(appUser, "normal");
                await _signInManager.SignInAsync(user, false); //<-- Sign in options.
                return await GenerateJwtToken(model.email, user);
            }

            string error = "USER_REGISTRATION_ERROR_";
            foreach (var e in result.Errors)
            {
                error = error + " *** " +e.Code +" - "+ e.Description;
            }
            return error;
        }

        private async Task<IdentityResult> AddToRole(IdentityUser user, string role) {
            IdentityResult roleResult = null;
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
            }

            var userResult = await _userManager.AddToRoleAsync(user, role);

            return roleResult;
        }

        private async Task<object> GenerateJwtToken(string email, IdentityUser user)
        {
            var encodedAPIKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var claims = new List<Claim> //<-- Claims options, check out JwtRegisteredClaimNames for more info.
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token  =  new JwtSecurityToken( //<--- Token options
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(180),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginDTO{
            [Required]
            public string email { get; set; }

            [Required]
            public string password { get; set; }
        }

        public class RegisterDTO {
            [Required]
            public string email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength =6)]
            public string password { get; set; }

            [Required]
            public string apikey { get; set; }
        }
    }
}
