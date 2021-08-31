
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectNotes.Models
{
public class Note
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public Author Author { get; set; }
    public int AuthorId { get; set; }
}
}