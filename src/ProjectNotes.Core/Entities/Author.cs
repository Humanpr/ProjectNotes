using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Core.Entities
{
    public class Author : BaseEntity
    {
        public string AuthorName { get; set; }
        public string NameIdentifier { get; set; }
        public string RegisterMethod { get; set; }
        public string accesstoken { get; set; }
        public string accesstokensecret { get; set; }
        public IEnumerable<Note>? Notes { get; set; } = new List<Note>();
    }
}
