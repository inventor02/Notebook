using Microsoft.AspNetCore.Identity;

namespace Notebook.Api.Models
{
    public class NotebookUser : IdentityUser<int>
    {
        public List<Note> Notes { get; set; } = null!;
    }
}
