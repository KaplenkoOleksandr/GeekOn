using Geekon.Data;
using Geekon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Geekon.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, GeekOnDBContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Statistic()
        {
            var projUs = _context.ProjectUsers.Where(p => p.UserId == _userManager.GetUserId(User)).Include(p => p.Project);

            var tasks = from t in _context.Tasks
                        join p in projUs on t.ProjectsProjId equals p.ProjectProjectId
                        where !p.Project.Archive && !t.Archive
                        select t;

            var subtasks = from s in _context.Subtasks
                           join t in tasks on s.TasksTaskId equals t.TaskId
                           where !s.Archive && s.ExecutorId == _userManager.GetUserId(User)
                           select s;

            double toDo = 0, inProg = 0, fin = 0, bugs = 0;
            
            foreach (var s in subtasks)
            {
                if (!s.Archive && s.Status == Models.TaskStatus.ToDo)
                    toDo++;
                if (!s.Archive && s.Status == Models.TaskStatus.InProgress)
                    inProg++;
                if (!s.Archive && s.Status == Models.TaskStatus.Finished)
                    fin++;
                if (!s.Archive && s.Status == Models.TaskStatus.Bugs)
                    bugs++;
            }

            Dictionary<Models.TaskStatus, double> stat = new Dictionary<Models.TaskStatus, double>();

            stat.Add(Models.TaskStatus.ToDo, toDo);
            stat.Add(Models.TaskStatus.InProgress, inProg);
            stat.Add(Models.TaskStatus.Finished, fin);
            stat.Add(Models.TaskStatus.Bugs, bugs);

            return View(stat);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
