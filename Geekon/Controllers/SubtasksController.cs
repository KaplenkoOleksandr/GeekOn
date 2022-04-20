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
    public class SubtasksController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubtasksController(GeekOnDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subtasks
        public async Task<IActionResult> Index(int? taskId)
        {
            if (taskId == null)
                return NotFound();

            var _subtaskContext = _context.Subtasks.Where(s => s.TasksTaskId == taskId);

            if (_subtaskContext == null)
                return NotFound();

            return View(_subtaskContext);
        }

        // GET: Subtasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subtasks = await _context.Subtasks.FirstOrDefaultAsync(s => s.SubtaskId == id);

            if (subtasks == null)
                return NotFound();

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == subtasks.TasksTaskId);

            var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == task.ProjectsProjId);

            var access = from ac in _context.ProjectUsers
                         where ac.UserId == _userManager.GetUserId(User) && ac.ProjectProjectId == proj.ProjectId
                         select ac;

            if (access.Count() == 0)
                return NoContent();

            return View(subtasks);
        }

        // GET: Subtasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subtasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int? taskId, [Bind("SubtaskId")] Subtasks subtasks)
        {
            try
            {
                subtasks.Status = Models.TaskStatus.ToDo;
                subtasks.SubtaskName = "New task";
                subtasks.TasksTaskId = (int)taskId;
                subtasks.Date = DateTime.Today;
                subtasks.Archive = false;
                _context.Add(subtasks);

                //add subtask into task
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
                task.Subtasks.Add(subtasks);
                //_context.Update(task);
                var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == task.ProjectsProjId);
                subtasks.ArchiveTaskId = proj.ArchiveTaskId;

                await _context.SaveChangesAsync();

                return View(subtasks);
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Subtasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subtasks = await _context.Subtasks.FindAsync(id);
            if (subtasks == null)
            {
                return NotFound();
            }
            return PartialView("_PartialSubtaskEdit", subtasks);
        }

        // POST: Subtasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("SubtaskId,SubtaskName,TasksTaskId,Status,ExecutorId,Date,Comment,Archive,ArchiveTaskId")] Subtasks subtasks)
        {
            try
            {
                if (subtasks.Archive)
                {
                    var arch = subtasks.ArchiveTaskId;
                    subtasks.ArchiveTaskId = subtasks.TasksTaskId;
                    subtasks.TasksTaskId = arch;
                }

                if(subtasks.ExecutorId == "iamexecutor")
                {
                    subtasks.ExecutorId = _userManager.GetUserId(User);
                }

                _context.Update(subtasks);
                await _context.SaveChangesAsync();
                return PartialView("_PartialSubtaskEdit", subtasks);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AnArchive(int subId)
        {
            try
            {
                var subtasks = await _context.Subtasks.AsNoTracking().FirstOrDefaultAsync(s => s.SubtaskId == subId);
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == subtasks.TasksTaskId);
                var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == task.ProjectsProjId);

                var arch = subtasks.ArchiveTaskId;
                subtasks.ArchiveTaskId = subtasks.TasksTaskId;
                subtasks.TasksTaskId = arch;
                subtasks.Archive = !subtasks.Archive;

                var archTask = await _context.Tasks.FirstOrDefaultAsync(a => a.TaskId == subtasks.TasksTaskId && !a.Archive);
                if (archTask == null)
                {
                    var firstTask = await _context.Tasks.FirstOrDefaultAsync(f => f.ProjectsProjId == proj.ProjectId && !f.Archive);
                    if (firstTask != null)
                        subtasks.TasksTaskId = firstTask.TaskId;
                    else
                    {
                        Tasks newTask = new Tasks();
                        newTask.TaskName = "New category";
                        newTask.ProjectsProjId = proj.ProjectId;
                        newTask.Projects = proj;
                        newTask.Archive = false;
                        newTask.Subtasks.Add(subtasks);
                        _context.Add(newTask);

                        subtasks.TasksTaskId = newTask.TaskId;
                    }
                }

                _context.Update(subtasks);
                await _context.SaveChangesAsync();
                return StatusCode(200);
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Subtasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subtasks = await _context.Subtasks
                .FirstOrDefaultAsync(m => m.SubtaskId == id);
            if (subtasks == null)
            {
                return NotFound();
            }

            return View(subtasks);
        }

        // POST: Subtasks/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subtasks = await _context.Subtasks.FindAsync(id);
            _context.Subtasks.Remove(subtasks);
            await _context.SaveChangesAsync();
            return View(subtasks);
        }

        private bool SubtasksExists(int id)
        {
            return _context.Subtasks.Any(e => e.SubtaskId == id);
        }
    }
}
