using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;
using Microsoft.AspNetCore.Identity;
using Geekon.Data;

namespace Geekon.Controllers
{
    public class TasksController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(GeekOnDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public TasksController()
        {
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? projId)
        {
            if (projId == null)
                return NotFound();

            ViewBag.projId = projId;

            var _taskContext = from t in _context.Tasks
                               where t.ProjectsProjId == projId && !t.Archive
                               select t;
            foreach (var t in _taskContext)
            {
                var _subtaskContext = from s in _context.Subtasks
                                      where s.TasksTaskId == t.TaskId && !s.Archive
                                      select s;

                foreach (var s in _subtaskContext.Distinct())
                    t.Subtasks.Add(s);
            }

            var access = from ac in _context.ProjectUsers
                         where ac.UserId == _userManager.GetUserId(User) && ac.ProjectProjectId == projId
                         select ac;

            if (access.Count() == 0)
                return NoContent();

            return PartialView("_PartialTest", _taskContext);

        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Create(int? projId, [Bind("TaskId")] Tasks tasks)
        {
            try
            {
                //Tasks tasks = new Tasks();
                tasks.TaskName = "New category";
                tasks.ProjectsProjId = (int)projId;
                tasks.Archive = false;
                _context.Add(tasks);
                _context.SaveChanges();

                //create subtask
                Subtasks subtask = new Subtasks();
                subtask.SubtaskName = "New task";
                subtask.Status = Models.TaskStatus.ToDo;
                subtask.TasksTaskId = tasks.TaskId;
                subtask.Date = DateTime.Today;
                subtask.Archive = false;
                _context.Add(subtask);



                tasks.Subtasks.Add(subtask);

                //_context.Update(tasks);

                //add task into proj
                var proj = _context.Projects.FirstOrDefault(p => p.ProjectId == projId);
                proj.Tasks.Add(tasks);
                subtask.ArchiveTaskId = proj.ArchiveTaskId;
                //_context.Update(proj);

                _context.SaveChanges();

                return View(tasks);
            }
            catch
            {
                return NotFound();
            }

        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int taskId, string taskName, bool archive = false)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.TaskId == taskId).FirstOrDefaultAsync();

                tasks.TaskName = taskName;

                _context.Update(tasks);
                await _context.SaveChangesAsync();
                return View(tasks);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AnArchive(int taskId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.TaskId == taskId).FirstOrDefaultAsync();

                tasks.Archive = false;

                _context.Update(tasks);
                await _context.SaveChangesAsync();
                return StatusCode(200);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Archive(int taskId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.TaskId == taskId).FirstOrDefaultAsync();

                tasks.Archive = true;

                _context.Update(tasks);
                await _context.SaveChangesAsync();
                return StatusCode(200);
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int taskId)
        {
            try
            {
                var tasks = _context.Tasks.Where(t => t.TaskId == taskId).Include(t => t.Subtasks);

                foreach (var s in tasks.FirstOrDefault().Subtasks)
                    if(!s.Archive)
                        _context.Subtasks.Remove(s);

                //delete task
                _context.Tasks.Remove(tasks.FirstOrDefault());
                _context.SaveChanges();
                return StatusCode(200);
            }
            catch (Exception e)
            {
                string a = e.Message;
                return NotFound();
            }
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
