﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public ICollection<Subtasks> Subtasks { get; set; }
    }
}
