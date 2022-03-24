using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class ProjectUsers
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string UserId { get; set; }

        public virtual Projects Project { get; set; }

        //user???
    }
}
