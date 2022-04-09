using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Subtasks
    {
        [Key]
        public int SubtaskId { get; set; }
        [Required]
        [Display(Name = "Task name")]
        public string SubtaskName { get; set; }
        public TaskStatus Status { get; set; }
        public string ExecutorId { get; set; } //Id of User, who will make it subtask
        [DataType(DataType.Date)]
        [Display(Name = "Deadline")]
        public DateTime Date { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public int TaskId { get; set; }
        public bool Archive { get; set; }

        public virtual Tasks Tasks { get; set; }
    }

    public enum TaskStatus
    { 
        ToDo,
        InProgress,
        Finished,
        Blocked
    }

}
