using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Models
{
    public class GeekOnDBContext : DbContext
    {

        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Subtasks> Subtasks { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<ProjectUsers> ProjectUsers { get; set; }

        public GeekOnDBContext(DbContextOptions<GeekOnDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
