using Microsoft.EntityFrameworkCore;
using ProjectNotes.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Infrastructure.Persistance
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasIndex(a => a.NameIdentifier).IsUnique();

            modelBuilder.Entity<Author>()
            .HasMany(a => a.Notes)
            .WithOne(n => n.Author)
            .HasForeignKey(n => n.AuthorId);


            modelBuilder.Entity<Note>().Property(n => n.Title).HasMaxLength(50);
            modelBuilder.Entity<Note>().Property(n => n.Text).HasMaxLength(280);
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Author> Authors { get; set; }

    }
}
