using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public ICollection<Subtask> Subtasks { get; set; }
    }
}
