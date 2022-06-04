using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notebook.Api.Data;
using Notebook.Api.Models;
using Notebook.Api.Services;

namespace Notebook.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly UserManager<NotebookUser> _userManager;
        private readonly INoteRepository _noteRepository;

        public NotesController(UserManager<NotebookUser> userManager, INoteRepository noteRepository)
        {
            _userManager = userManager;
            _noteRepository = noteRepository;
        }

        // GET: api/Notes
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            var user = await _userManager.GetUserAsync(User);
            var notes = await _noteRepository.GetNotes(user);

            return Ok(notes);
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Note>> GetNote(long id)
        {
            var note = await _noteRepository.GetNote(id);
            var user = await _userManager.GetUserAsync(User);

            if (note == null)
            {
                return NotFound();
            }

            if (!note.Public)
            {
                if (user == null || note.CreatorId != user.Id)
                {
                    return Unauthorized();
                }
            }

            return Ok(note);
        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(long id, [Bind("Id,Content,Public")] Note note)
        {
            if (id != note.Id)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);

            if (note.CreatorId != user.Id)
            {
                return Unauthorized();
            }

            await _noteRepository.UpdateNote(note);
            return NoContent();
        }

        // POST: api/Notes
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote([Bind("Content,Public")] Note note)
        {
            var user = await _userManager.GetUserAsync(User);

            note.CreatorId = user.Id;
            note.CreatedAt = DateTime.Now;

            await _noteRepository.CreateNote(note);

            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(long id)
        {
            var note = await _noteRepository.GetNote(id);
            var user = await _userManager.GetUserAsync(User);

            if (note == null)
            {
                return NotFound();
            }

            if (note.CreatorId != user.Id)
            {
                return Unauthorized();
            }

            await _noteRepository.DeleteNote(note);
            return NoContent();
        }
    }
}
