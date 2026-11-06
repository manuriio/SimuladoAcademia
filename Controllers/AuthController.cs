using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoAcademia.Data;
using SimuladoAcademia.Models;

namespace SimuladoAcademia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;


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


            return Ok(new { usuario.Nome, usuario.Username });
        }
    }
}
