namespace SimuladoAcademia.Models
{
    public class AlunoDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }


        public int PersonalId { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; }
    }


    public class AlunoCreateDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int PersonalId { get; set; }
    }


    public class AlunoUpdateDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int PersonalId { get; set; }
    }
}
