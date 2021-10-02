using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Core.Entities
{
    public class Note : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }
    }
}
