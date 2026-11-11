using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoAcademia.Data;
using SimuladoAcademia.Models;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class PersonalController : ControllerBase
{
    private readonly AppDbContext _context;


    private static readonly string EncryptionKey = "12345678901234567890123456789012";
    private static readonly string EncryptionIV = "1234567890123456";


    public PersonalController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonalDto>>> GetPersonais([FromQuery] string? busca)
    {
        var personais = await _context.Personal.AsNoTracking().ToListAsync();


        var lista = personais.Select(p => new PersonalDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Cpf = DescriptografarCpf(p.Cpf),
            Email = p.Email
        });


        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            var termoNumerico = new string(termo.Where(char.IsDigit).ToArray());


            lista = lista.Where(p =>
                (p.Nome ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                (p.Email ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                (p.Cpf ?? "").Contains(termo) ||
                (termoNumerico.Length > 0 && (p.Cpf ?? "").Replace(".", "").Replace("-", "").Contains(termoNumerico)));
        }


        return Ok(lista.OrderBy(p => p.Nome).ToList());
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<PersonalDto>> GetPersonal(int id)
    {
        var p = await _context.Personal.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();


        return Ok(new PersonalDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Cpf = DescriptografarCpf(p.Cpf),
            Email = p.Email
        });
    }


    [HttpPost]
    public async Task<ActionResult<PersonalDto>> CreatePersonal(PersonalCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Cpf) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Nome, CPF e e-mail são obrigatórios.");


        if (await CpfJaCadastrado(dto.Cpf))
            return Conflict("Já existe um Personal com este CPF.");


        var personal = new Personal
        {
            Nome = dto.Nome,
            Cpf = CriptografarCpf(dto.Cpf),
            Email = dto.Email
        };


        _context.Personal.Add(personal);
        await _context.SaveChangesAsync();


        var personalDto = new PersonalDto
        {
            Id = personal.Id,
            Nome = personal.Nome,
            Cpf = dto.Cpf,
            Email = personal.Email
        };


        return CreatedAtAction(nameof(GetPersonal), new { id = personal.Id }, personalDto);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePersonal(int id, PersonalUpdateDto dto)
    {
        var personal = await _context.Personal.FindAsync(id);
        if (personal == null) return NotFound();


        if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Cpf) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Nome, CPF e e-mail são obrigatórios.");


        if (await CpfJaCadastrado(dto.Cpf, id))
            return Conflict("Já existe outro Personal com este CPF.");


        personal.Nome = dto.Nome;
        personal.Cpf = CriptografarCpf(dto.Cpf);
        personal.Email = dto.Email;


        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePersonal(int id)
    {
        var personal = await _context.Personal.FindAsync(id);
        if (personal == null) return NotFound();


        bool temVinculos =
            await _context.Alunos.AnyAsync(a => a.PersonalId == id) ||
            await _context.Agendamentos.AnyAsync(a => a.PersonalId == id);


        if (temVinculos)
            return BadRequest("Não é possível excluir: o Personal possui alunos ou agendamentos vinculados.");


        _context.Personal.Remove(personal);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    private async Task<bool> CpfJaCadastrado(string cpf, int? ignorarId = null)
    {
        var cpfCripto = CriptografarCpf(cpf);
        return await _context.Personal.AnyAsync(p => p.Cpf == cpfCripto && (ignorarId == null || p.Id != ignorarId));
    }


    private static string CriptografarCpf(string cpf)
    {
        if (string.IsNullOrEmpty(cpf)) return cpf;


        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(EncryptionIV);


        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(cpf);
        }
        return Convert.ToBase64String(ms.ToArray());
    }


    private static string DescriptografarCpf(string cpfCriptografado)
    {
        if (string.IsNullOrEmpty(cpfCriptografado)) return cpfCriptografado;


        try
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = Encoding.UTF8.GetBytes(EncryptionIV);


            using var ms = new MemoryStream(Convert.FromBase64String(cpfCriptografado));
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        catch
        {
            return cpfCriptografado;
        }
    }
}