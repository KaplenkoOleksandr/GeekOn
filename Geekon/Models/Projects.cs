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
        public string CreatorId { get; set; } // user id
        [Required]
        [Display(Name = "Name of project")]
        public string ProjName { get; set; }
        [DataType(DataType.Date)]
        public DateTimeOffset DateCreate { get; set; }
        [Display(Name = "Project image")]
        public string ProjImagePath { get; set; }
        public string ProjFolderLink { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
