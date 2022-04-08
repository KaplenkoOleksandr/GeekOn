using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Tasks
    {
        public Tasks()
        {
            Subtasks = new List<Subtasks>();
        }
        [Key]
        public int TaskId { get; set; }
        [Required]
        [Display(Name = "Task group name")]
        public string TaskName { get; set; }
        public int ProjId { get; set; }

        public virtual Projects Projects { get; set; }
        public virtual ICollection<Subtasks> Subtasks { get; set; }
    }
}
