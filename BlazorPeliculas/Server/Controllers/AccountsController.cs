using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorPeliculas.Server.Controllers {
    [ApiController, Route("api/accounts")]
    public class AccountsController: ControllerBase {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountsController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromBody] UserInfoDTO model) {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if(result.Succeeded) {
                return await BuildToken(model);
            }
            else
                return BadRequest(result.Errors.First());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserInfoDTO model) {
            //isPersistent: lo guarda en cookies
            //lockoutOnFailure: se lockea el usuario después varios intentos incorrectos.
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
                return await BuildToken(model);
            else
                return BadRequest("User/Password incorrect");
        }

        [HttpGet("renewToken")]
        public async Task<ActionResult<UserTokenDTO>> Renew() {
            //Construimos un userInfo para poder llamar a BuildToken que recibe uno como parámetro.
            var userInfo = new UserInfoDTO() {
                Email = HttpContext.User.Identity!.Name!
            };

            return await BuildToken(userInfo);
        }

        private async Task<UserTokenDTO> BuildToken(UserInfoDTO userInfo) {
            var claims = new List<Claim>() {
                //Esta info es accesible desde el frontend de Blazor. NO VA NINGÚN DATO SENSIBLE (clave, tarjetas de crédito, etc)
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim("miValor", "Lo qu necesite...")
            };

            var user = await userManager.FindByEmailAsync(userInfo.Email);
            var roles = await userManager.GetRolesAsync(user!);

            foreach(var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new UserTokenDTO {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
