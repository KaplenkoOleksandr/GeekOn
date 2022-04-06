using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;

namespace Geekon.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProjectsController(GeekOnDBContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _projContext = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);

            if (_projContext == null)
                return NotFound();

            var access = from ac in _context.ProjectUsers
                       where ac.UserId == _userManager.GetUserId(User) && ac.ProjectId == id
                       select ac;

            if (access.Count() == 0)
                return NoContent();

            return View(_projContext);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            var provider = new PhysicalFileProvider(_env.WebRootPath);
            var contents = provider.GetDirectoryContents("projBack");
            var objFiles = contents.OrderBy(m => m.LastModified);

            List<string> projBack = new List<string>();
            foreach (var item in objFiles.ToList())
            {
                projBack.Add(item.Name);
            }
            ViewData["projBack"] = projBack;
            return PartialView("_ProjectCreatePartial");
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,ProjName,ProjImagePath")] Projects projects)
        {
            projects.CreatorId = _userManager.GetUserId(User);
            projects.DateCreate = DateTimeOffset.Now;
            if (ModelState.IsValid)
            {
                // add Project-User row
                ProjectUsers projectUser = new ProjectUsers();
                projectUser.Project = projects;
                projectUser.ProjectId = projects.ProjectId;
                projectUser.UserId = projects.CreatorId;
                _context.ProjectUsers.Add(projectUser);

                _context.Add(projects);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projects);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects.FindAsync(id);
            if (projects == null)
            {
                return NotFound();
            }
            return View(projects);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjName,ProjImagePath")] Projects projects)
        {
            if (id != projects.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projects);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectsExists(projects.ProjectId))
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
            return View(projects);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projects = await _context.Projects.FindAsync(id);

            //delete all connetions project with users
            var projUsers = from pu in _context.ProjectUsers
                            where pu.ProjectId == id
                            select pu;
            foreach (var pu in projUsers)
                _context.ProjectUsers.Remove(pu);

            //delete all tasks from project
            foreach (var t in projects.Tasks)
                _context.Tasks.Remove(t);

            //delete project
            _context.Projects.Remove(projects);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectsExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
