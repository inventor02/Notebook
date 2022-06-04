using Notebook.Api.Models;

namespace Notebook.Api.Services
{
    public interface INoteRepository
    {
        public Task<IEnumerable<Note>> GetNotes(NotebookUser? user = null);
        public Task<Note?> GetNote(long id);
        public Task CreateNote(Note note);
        public Task UpdateNote(Note note);
        public Task DeleteNote(Note note);
    }
}
