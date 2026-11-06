namespace SimuladoAcademia.Models
{
    public class Agendamento
    {
        public int id { get; set; }
        public DateTime? DataHora { get; set; }
        public string? Servico { get; set; }
        public int AlunoId { get; set; }
        public Aluno? Aluno { get; set; }

        public int PersonalId { get; set; }
        public Personal? Personal { get; set; }
    }
}
