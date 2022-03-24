using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Projects
    {
        public int ProjectId { get; set; }
        public string Creator { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
