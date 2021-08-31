using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectNotes.Models
{
public class Author
{
    [Key]
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public string NameIdentifier { get; set; }
    public IEnumerable<Note>? Notes { get; set; }
}

}