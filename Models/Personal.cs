namespace SimuladoAcademia.Models
{
    public class Personal
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public ICollection<Aluno>? Alunos { get; set; }

        public ICollection<Agendamento>? Agendamentos { get; set; }

    }
}
