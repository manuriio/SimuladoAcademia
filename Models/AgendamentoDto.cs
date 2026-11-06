namespace SimuladoAcademia.Models
{
    public class AgendamentoDto
    {
        public int id { get; set; }
        public DateTime? DataHora { get; set; }
        public string Servico { get; set; }
        public int AlunoId { get; set; }
        public int PersonalId { get; set; }
    }

    public class AgendamentoCreateDto
    {
        public DateTime DataHora { get; set; }
        public string? Servico { get; set; }
        public int AlunoId { get; set; }
        public int PersonalId { get; set; }
    }

    public class AgentamentoUpdateDto
    {
        public DateTime DataHora { get; set; }
        public string? Servico { get; set; }
        public int AlunoId { get; set; }
        public int PersonalId { get; set; }
    }
}
