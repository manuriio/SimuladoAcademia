namespace SimuladoAcademia.Models
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }

        public int PersonalId { get; set; }
        public Personal? Personal { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }
    }
}
