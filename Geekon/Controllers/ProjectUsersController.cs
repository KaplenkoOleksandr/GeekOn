using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;

namespace Geekon.Controllers
{
    public class ProjectUsersController : Controller
    {
        private readonly GeekOnDBContext _context;

        public ProjectUsersController(GeekOnDBContext context)
        {
            _context = context;
        }

        // GET: ProjectUsers
        public async Task<IActionResult> Index(string userId)
        {
            var geekOnDBContext = _context.ProjectUsers.Where(u => u.UserId == userId).Include(p => p.Project);
            return View(await geekOnDBContext.ToListAsync());
        }

        // GET: ProjectUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsers = await _context.ProjectUsers
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUsers == null)
            {
                return NotFound();
            }

            return View(projectUsers);
        }

        // GET: ProjectUsers/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");
            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,UserId")] ProjectUsers projectUsers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectUsers.ProjectId);
            return View(projectUsers);
        }

        // GET: ProjectUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsers = await _context.ProjectUsers.FindAsync(id);
            if (projectUsers == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectUsers.ProjectId);
            return View(projectUsers);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UserId")] ProjectUsers projectUsers)
        {
            if (id != projectUsers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectUsersExists(projectUsers.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectUsers.ProjectId);
            return View(projectUsers);
        }

        // GET: ProjectUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsers = await _context.ProjectUsers
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUsers == null)
            {
                return NotFound();
            }

            return View(projectUsers);
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectUsers = await _context.ProjectUsers.FindAsync(id);
            _context.ProjectUsers.Remove(projectUsers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectUsersExists(int id)
        {
            return _context.ProjectUsers.Any(e => e.Id == id);
        }
    }
}
