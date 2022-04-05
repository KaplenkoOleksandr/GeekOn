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
    public class SubtasksController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubtasksController(GeekOnDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subtasks
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return NotFound();

            var _subtaskContext = await _context.Subtasks.FirstOrDefaultAsync(s => s.SubtaskId == id);

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

            var task = from t in _context.Tasks
                       where t.Subtasks.Contains(subtasks)
                       select t;

            var proj = from p in _context.Projects
                       where p.Tasks.Contains(task.FirstOrDefault())
                       select p;

            var access = from ac in _context.ProjectUsers
                         where ac.UserId == _userManager.GetUserId(User) && ac.ProjectId == proj.FirstOrDefault().ProjectId
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubtaskId,SubtaskName,Date,Comment")] Subtasks subtasks, int? taskId)
        {
            subtasks.Status = Models.TaskStatus.ToDo;
            if (ModelState.IsValid)
            {
                _context.Add(subtasks);

                //add subtask into task
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
                task.Subtasks.Add(subtasks);
                _context.Update(task);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subtasks);
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
            return View(subtasks);
        }

        // POST: Subtasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubtaskId,SubtaskName,Status,UserId,Date,Comment")] Subtasks subtasks)
        {
            if (id != subtasks.SubtaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subtasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubtasksExists(subtasks.SubtaskId))
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
            return View(subtasks);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subtasks = await _context.Subtasks.FindAsync(id);
            _context.Subtasks.Remove(subtasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubtasksExists(int id)
        {
            return _context.Subtasks.Any(e => e.SubtaskId == id);
        }
    }
}
