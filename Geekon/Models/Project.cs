using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Creator { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
