using Microsoft.EntityFrameworkCore;
using Notebook.Api.Data;
using Notebook.Api.Models;

namespace Notebook.Api.Services
{
    public class NoteRepository : INoteRepository
    {
        private readonly NotebookContext _context;

        public NoteRepository(NotebookContext context)
        {
            _context = context;
        }

        public async Task CreateNote(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNote(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }

        public async Task<Note?> GetNote(long id)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
            return note;
        }

        public async Task<IEnumerable<Note>> GetNotes(NotebookUser? user = null)
        {
            var query = from note in _context.Notes
                        where note.Public
                        select note;
            var notes = await query.ToListAsync();

            if (user != null)
            {
                var userNotesQuery = from note in _context.Notes
                                where !note.Public && note.CreatorId == user.Id
                                select note;
                var userNotes = await userNotesQuery.ToListAsync();

                notes = (List<Note>) notes.Union(userNotes);
            }

            return notes;
        }

        public async Task UpdateNote(Note note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }
    }
}
