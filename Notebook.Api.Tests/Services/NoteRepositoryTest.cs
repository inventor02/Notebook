using Microsoft.EntityFrameworkCore;
using Notebook.Api.Data;
using Notebook.Api.Models;
using Notebook.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Api.Tests.Services
{
    public class NoteRepositoryTest
    {
        private NotebookContext context;

        private NoteRepository noteRepository;

        public NoteRepositoryTest()
        {
            var contextOptions = new DbContextOptionsBuilder<NotebookContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            context = new NotebookContext(contextOptions);

            noteRepository = new NoteRepository(context);
        }

        [Fact]
        public async Task CreateNote_Creates_Note()
        {
            // Arrange
            var note = new Note()
            {
                Content = "Test Note",
                CreatorId = 1,
                CreatedAt = DateTime.Now,
            };

            // Act
            await noteRepository.CreateNote(note);

            // Assert
            Assert.True(context.Notes.Count() == 1);
            Assert.True(context.Notes.Where(n => n.CreatorId == 1).Count() == 1);
        }

        [Fact]
        public async Task DeleteNote_Deletes_Note()
        {
            // Arrange
            var note = new Note()
            {
                Content = "Test Note",
                CreatorId = 1,
                CreatedAt = DateTime.Now
            };
            note = context.Add(note).Entity;
            context.SaveChanges();

            // Act
            await noteRepository.DeleteNote(note);

            // Assert
            Assert.True(context.Notes.Count() == 0);
        }

        [Fact]
        public async Task UpdateNote_Updates_Note()
        {
            // Arrange
            var note = new Note()
            {
                Content = "Test Note",
                CreatorId = 1,
                CreatedAt = DateTime.Now
            };
            note = context.Add(note).Entity;
            context.SaveChanges();

            // Act
            note.Content = "Updated";
            await noteRepository.UpdateNote(note);

            // Assert
            var dbNote = context.Notes.Find(note.Id)!;
            Assert.True(dbNote.Content == "Updated");
        }

        [Fact]
        public async Task GetNote_Gets_Note()
        {
            // Arrange
            var note = new Note()
            {
                Content = "Test Note",
                CreatorId = 1,
                CreatedAt = DateTime.Now
            };
            note = context.Add(note).Entity;
            context.SaveChanges();

            // Act
            var gotNote = await noteRepository.GetNote(note.Id);

            // Assert
            Assert.NotNull(gotNote);
            Assert.Equal(note.Id, gotNote!.Id);
        }

        [Fact]
        public async Task GetNote_With_Invalid_Id_Returns_Null()
        {
            // Arrange
            var note = new Note()
            {
                Content = "Test Note",
                CreatorId = 1,
                CreatedAt = DateTime.Now
            };
            context.Add(note);
            context.SaveChanges();

            // Act
            var gotNote = await noteRepository.GetNote(note.Id + 1);

            // Assert
            Assert.Null(gotNote);
        }

        [Fact]
        public async Task GetNotes_Returns_All_Public_Notes()
        {
            // Public Notes
            for (int i = 0; i < 10; i++)
            {
                var note = new Note()
                {
                    Content = "Test Note " + i,
                    CreatorId = 1,
                    CreatedAt = DateTime.Now,
                    Public = true
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            // Private Notes
            for (int i = 10; i < 20; i++)
            {
                var note = new Note()
                {
                    Content = "Test Note " + i,
                    CreatorId = 1,
                    CreatedAt = DateTime.Now,
                    Public = false
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            var notes = await noteRepository.GetNotes();

            Assert.Equal(10, notes.Count());

            foreach (var note in notes)
            {
                Assert.True(note.Public, "Notes must be public");
            }
        }

        [Fact]
        public async Task GetNotes_With_User_Returns_Correct_Notes()
        {
            // Users
            var user1 = new NotebookUser()
            {
                Email = "user1@test.com",
                UserName = "user1@test.com"
            };
            var user2 = new NotebookUser()
            {
                Email = "user2@test.com",
                UserName = "user2@test.com"
            };
            context.Add(user1);
            context.Add(user2);
            context.SaveChanges();

            // Public notes x5
            for (int i = 5; i < 10; i++)
            {
                var note = new Note()
                {
                    Content = "Test Note " + i,
                    CreatorId = user1.Id,
                    CreatedAt = DateTime.Now,
                    Public = true
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            // Private notes for user 1 x2
            for (int i = 2; i < 10; i++)
            {
                var note = new Note()
                {
                    Content = "U1 Private Note " + i,
                    CreatorId = user1.Id,
                    CreatedAt = DateTime.Now,
                    Public = false
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            // Private notes for user 2 x2
            for (int i = 2; i < 10; i++)
            {
                var note = new Note()
                {
                    Content = "U2 Private Note " + i,
                    CreatorId = user1.Id,
                    CreatedAt = DateTime.Now,
                    Public = false
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            var notes = await noteRepository.GetNotes(user1);

            Assert.Equal(12, notes.Count());

            foreach (var note in notes)
            {
                Assert.True(note.Public || note.CreatorId == user1.Id, "Notes must be public or belong to this user");
            }
        }
    }
}
