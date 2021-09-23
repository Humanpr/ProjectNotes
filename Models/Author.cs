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
    public string RegisterMethod { get; set; }
    public string accesstoken { get; set; }
    public string accesstokensecret { get; set; }
    public IEnumerable<Note>? Notes { get; set; } = new List<Note>();
}

}