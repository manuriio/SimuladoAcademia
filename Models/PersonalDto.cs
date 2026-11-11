namespace SimuladoAcademia.Models
{ 
   public class PersonalDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
    public ICollection<Aluno>? Alunos { get; set; }


    public ICollection<Agendamento>? Agendamentos { get; set; }
}


public class PersonalCreateDto
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
}


public class PersonalUpdateDto
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
}
}