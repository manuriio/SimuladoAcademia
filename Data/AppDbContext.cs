using Microsoft.EntityFrameworkCore;
using SimuladoAcademia.Models;

namespace SimuladoAcademia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Usuario> Usuarios { get; set; }
        public DbSet<Models.Personal> Personal { get; set; }
        public DbSet<Models.Aluno> Alunos { get; set; }
        public DbSet<Models.Agendamento> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Desativa a exclusão em cascata entre Agendamento e Personal
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Personal)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(a => a.PersonalId)
                .OnDelete(DeleteBehavior.Restrict); // ou DeleteBehavior.NoAction


            // Mantenha o relacionamento de Aluno com Agendamento padrão
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Aluno)
                .WithMany(al => al.Agendamentos)
                .HasForeignKey(a => a.AlunoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
