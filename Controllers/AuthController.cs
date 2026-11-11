using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimuladoAcademia.Data;
using SimuladoAcademia.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimuladoAcademia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;


        // Definindo uma chave secreta para a assinatura do token (Mínimo de 32 caracteres)
        public static readonly string JwtSecretKey = "ChaveSecretaMuitoSeguraParaOJwtSimuladoIronFit123!";


        public AuthController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username.ToLower() == registerDto.Username.ToLower()))
            {
                return BadRequest("Este username já está em uso.");
            }


            string senhaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);


            var novoUsuario = new Usuario
            {
                Nome = registerDto.Nome,
                Username = registerDto.Username,
                Password = senhaHash
            };


            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(Register), new { id = novoUsuario.Id }, new { novoUsuario.Nome, novoUsuario.Username });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto authDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username.ToLower() == authDto.Username.ToLower());


            if (usuario == null || !BCrypt.Net.BCrypt.Verify(authDto.Password, usuario.Password))
            {
                return Unauthorized("Credenciais inválidas.");
            }


            // --- GERAÇÃO DO TOKEN JWT ---
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecretKey);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Username", usuario.Username)
            }),
                Expires = DateTime.UtcNow.AddHours(2), // Expira em 2 horas (Atende o RNF de tempo limite)
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            // Retorna o token junto com os dados básicos do usuário
            return Ok(new
            {
                Token = tokenString,
                Usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Username
                }
            });
        }
    }
}
