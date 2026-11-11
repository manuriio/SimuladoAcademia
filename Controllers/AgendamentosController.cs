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
    public class AgendamentosController : ControllerBase
    {
        private readonly AppDbContext _context;


        public AgendamentosController
            (AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAgendamentos()
        {
            var lista = await _context.Agendamentos
                .AsNoTracking()
                .Include(a => a.Personal)
                .Include(a => a.Aluno)
                .OrderBy(a => a.DataHora)
                .ToListAsync();


            var resposta = lista.Select(a => new
            {
                a.id,
                a.DataHora,
                a.Servico,
                Personal = new
                {
                    a.Personal.Id,
                    a.Personal.Nome,
                    a.Personal.Email
                },
                Aluno = new
                {
                    a.Aluno.Id,
                    a.Aluno.Nome,
                    a.Aluno.Email,
                    a.Aluno.Telefone
                }
            });


            return Ok(resposta);
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> CreateAgendamento(AgendamentoCreateDto agendamentoCreateDto)
        {
            // Verifica se o aluno existe
            var aluno = await _context.Alunos
                .FindAsync(agendamentoCreateDto.AlunoId);

            if (aluno == null)
            {
                return BadRequest("Aluno não encontrado.");
            }

            // Verifica se o personal existe
            var personal = await _context.Personal
                .FindAsync(agendamentoCreateDto.PersonalId);

            if (personal == null)
            {
                return BadRequest("Personal não encontrado.");
            }

            var agendamento = new Agendamento
            {
                DataHora = agendamentoCreateDto.DataHora,
                PersonalId = agendamentoCreateDto.PersonalId,
                Servico = agendamentoCreateDto.Servico,
                AlunoId = agendamentoCreateDto.AlunoId
            };

            _context.Agendamentos.Add(agendamento);

            await _context.SaveChangesAsync();

            var agendamentoDto = new AgendamentoDto
            {
                id = agendamento.id,
                DataHora = agendamento.DataHora,
                PersonalId = agendamento.PersonalId,
                AlunoId = agendamento.AlunoId
            };

            return Ok(agendamentoDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgendamento(int id, AgentamentoUpdateDto agendamentoUpdateDto)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
            {
                return NotFound();
            }
            agendamento.DataHora = agendamentoUpdateDto.DataHora;
            agendamento.PersonalId = agendamentoUpdateDto.PersonalId;
            agendamento.AlunoId = agendamentoUpdateDto.AlunoId;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgendamento(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
            {
                return NotFound();
            }
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
