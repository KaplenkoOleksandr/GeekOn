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
        public string SubtaskName { get; set; }
        public TaskStatus Status { get; set; }

        public int UserId { get; set; }
        //public ICollection<User> Users { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }

    public enum TaskStatus
    { 
        ToDo,
        InProgress,
        Finished,
        Blocked
    }

}
