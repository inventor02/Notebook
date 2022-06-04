using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notebook.Api.Models
{
    public class Note
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public NotebookUser Creator { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool Public { get; set; } = true;
    }
}
