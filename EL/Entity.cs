using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EL
{
    public class Entity
    {
        [NotMapped]
        public bool IsSuccess { get; set; }

        [NotMapped]
        public List<string> MessageList { get; set; } = new List<string>();
    }
}