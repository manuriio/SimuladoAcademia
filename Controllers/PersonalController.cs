using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoAcademia.Data;
using SimuladoAcademia.Models;

namespace SimuladoAcademia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonalController : ControllerBase
    {
        private readonly AppDbContext _context;


        public PersonalController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalDto>>> GetPersonais()
        {
            return await _context.Personal.Select(p => new PersonalDto
            {
                Nome = p.Nome,
                Cpf = p.Cpf,
                Email = p.Email,
                Alunos = p.Alunos,
                Agendamentos = p.Agendamentos


            }).ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<PersonalDto>> CreatePersonal(PersonalCreateDto personalCreateDto)
        {
            var personal = new Personal
            {
                Nome = personalCreateDto.Nome,
                Cpf = personalCreateDto.Cpf,
                Email = personalCreateDto.Email
            };
            _context.Personal.Add(personal);
            await _context.SaveChangesAsync();
            var personalDto = new PersonalDto
            {
                Nome = personal.Nome,
                Cpf = personal.Cpf,
                Email = personal.Email,
                Alunos = personal.Alunos,
                Agendamentos = personal.Agendamentos
            };
            return CreatedAtAction(nameof(GetPersonais), new { id = personal.Id }, personalDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonal(int id, PersonalUpdateDto personalUpdateDto)
        {
            var personal = await _context.Personal.FindAsync(id);
            if (personal == null)
            {
                return NotFound();
            }
            personal.Nome = personalUpdateDto.Nome;
            personal.Cpf = personalUpdateDto.Cpf;
            personal.Email = personalUpdateDto.Email;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonal(int id)
        {
            var personal = await _context.Personal.FindAsync(id);
            if (personal == null)
            {
                return NotFound();
            }
            _context.Personal.Remove(personal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
