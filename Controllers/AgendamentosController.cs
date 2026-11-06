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
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetAgendamentos()
        {
            return await _context.Agendamentos.Select(a => new AgendamentoDto
            {
                id = a.id,
                DataHora = a.DataHora,
                PersonalId = a.PersonalId,
                AlunoId = a.AlunoId
            }).ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> CreateAgendamento(AgendamentoCreateDto agendamentoCreateDto)
        {
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
            return CreatedAtAction(nameof(GetAgendamentos), new { id = agendamento.id }, agendamentoDto);
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
