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
        public string ProjName { get; set; }
        public string ProjImagePath { get; set; }
        public string ProjFolderLink { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
