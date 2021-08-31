using System;
using Microsoft.EntityFrameworkCore;
using ProjectNotes.Models;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options):base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasIndex(a=>a.NameIdentifier).IsUnique();

        modelBuilder.Entity<Author>()
        .HasMany(a=>a.Notes)
        .WithOne(n=>n.Author)
        .HasForeignKey(n=>n.AuthorId);
    }

    public DbSet<Note> Notes {get;set;}
    public DbSet<Author> Authors {get;set;}
    
}