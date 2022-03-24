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
    public class SubtasksController : Controller
    {
        private readonly GeekOnDBContext _context;

        public SubtasksController(GeekOnDBContext context)
        {
            _context = context;
        }

        // GET: Subtasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subtasks.ToListAsync());
        }

        // GET: Subtasks/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Create([Bind("SubtaskId,SubtaskName,Status,UserId,Date,Comment")] Subtasks subtasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subtasks);
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
