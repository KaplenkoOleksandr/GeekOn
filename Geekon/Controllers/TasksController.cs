using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;
using Microsoft.AspNetCore.Identity;

namespace Geekon.Controllers
{
    public class TasksController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(GeekOnDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return PartialView("_PartialTest");

            var _taskContext = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);

            if (_taskContext == null)
                return NotFound();

            var proj = from p in _context.Projects
                       where p.Tasks.Contains(_taskContext)
                       select p;

            var access = from ac in _context.ProjectUsers
                       where ac.UserId == _userManager.GetUserId(User) && ac.ProjectId == proj.FirstOrDefault().ProjectId 
                       select ac;

            if (access.Count() == 0)
                return NoContent();

            //return View(_taskContext);
            return PartialView("_PartialTest");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,TaskName")] Tasks tasks, int? projId)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);

                //add task into proj
                var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projId);
                proj.Tasks.Add(tasks);
                _context.Update(proj);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskName")] Tasks tasks)
        {
            if (id != tasks.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);

            //delete all subtasks from task
            foreach (var s in tasks.Subtasks)
                _context.Subtasks.Remove(s);
            //delete task
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
