using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Projects
    {
        [Key]
        public int ProjectId { get; set; }
        public int CreatorId { get; set; } // user id
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
