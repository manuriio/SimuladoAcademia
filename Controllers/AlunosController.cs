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
    public class AlunosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlunosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlunoDto>>> GetAlunos()
        {
            return await _context.Alunos.Select(a => new AlunoDto
            {
                Nome = a.Nome,
                Email = a.Email,
                Telefone = a.Telefone,
                PersonalId = a.PersonalId,
                Agendamentos = a.Agendamentos
            }).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<AlunoDto>> CreateAluno(AlunoCreateDto alunoCreateDto)
        {
            var aluno = new Aluno
            {
                Nome = alunoCreateDto.Nome,
                Email = alunoCreateDto.Email,
                Telefone = alunoCreateDto.Telefone,
                PersonalId = alunoCreateDto.PersonalId
            };
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            var alunoDto = new AlunoDto
            {
                Nome = aluno.Nome,
                Email = aluno.Email,
                Telefone = aluno.Telefone,
                PersonalId = aluno.PersonalId,
                Agendamentos = aluno.Agendamentos
            };
            return CreatedAtAction(nameof(GetAlunos), new { id = aluno.Id }, alunoDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAluno(int id, AlunoUpdateDto alunoUpdateDto)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }


            aluno.Nome = alunoUpdateDto.Nome;
            aluno.Email = alunoUpdateDto.Email;
            aluno.Telefone = alunoUpdateDto.Telefone;
            aluno.PersonalId = alunoUpdateDto.PersonalId;


            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluno(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}