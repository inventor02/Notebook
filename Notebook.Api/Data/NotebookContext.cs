using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notebook.Api.Models;

namespace Notebook.Api.Data
{
    public class NotebookContext : IdentityDbContext<NotebookUser, NotebookRole, int>
    {
        public DbSet<Note> Notes { get; set; } = null!;

        public NotebookContext(DbContextOptions<NotebookContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();
    }
}
