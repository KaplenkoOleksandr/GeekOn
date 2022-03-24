using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Subtasks
    {
        public int SubtaskId { get; set; }
        public string SubtaskName { get; set; }
        public TaskStatus Status { get; set; }
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
