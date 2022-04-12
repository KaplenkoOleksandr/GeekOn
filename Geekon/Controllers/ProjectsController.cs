﻿using System;
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
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Util.Store;

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

        public IActionResult SomeAct(int? id)
        {
            return PartialView("_PartialTest");
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                //return View();
                return NotFound();

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
        public async Task<IActionResult> Details(int? projId)
        {
            if (projId == null)
            {
                return NotFound();
            }

            var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projId);
            var archTask = await _context.Tasks.FirstOrDefaultAsync(a => a.ProjId == projId && a.TaskId == proj.ArchiveTaskId);

            List<Tasks> archiveTasks = new List<Tasks>();
            archiveTasks.Add(archTask);

            var tasks = from t in _context.Tasks
                        where t.Archive
                        select t;

            foreach(var t in tasks)
            {
                archiveTasks.Add(t);
            }

            return View(archiveTasks);
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
            try
            {
                projects.CreatorId = _userManager.GetUserId(User);
                projects.DateCreate = DateTimeOffset.Now;
                projects.Archive = false;
                _context.Add(projects);
                _context.SaveChanges();

                // add Project-User row
                ProjectUsers projectUser = new ProjectUsers();
                projectUser.Project = projects;
                projectUser.ProjectId = projects.ProjectId;
                projectUser.UserId = projects.CreatorId;
                _context.ProjectUsers.Add(projectUser);
                _context.SaveChanges();

                // add first Task into new project
                Tasks tasks = new Tasks();
                tasks.TaskName = "New category";
                tasks.ProjId = projects.ProjectId;
                tasks.Archive = false;
                _context.Add(tasks);
                _context.SaveChanges();
                projects.Tasks.Add(tasks);

                // add first Subask into new project
                Subtasks subtask = new Subtasks();
                subtask.SubtaskName = "New task";
                subtask.Status = Models.TaskStatus.ToDo;
                subtask.TaskId = tasks.TaskId;
                subtask.Date = DateTime.Today;
                subtask.Archive = false;
                _context.Add(subtask);
                _context.SaveChanges();
                tasks.Subtasks.Add(subtask);

                // add archive Task into new project
                Tasks archTasks = new Tasks();
                archTasks.TaskName = "Archive";
                archTasks.ProjId = projects.ProjectId;
                archTasks.Archive = true;
                _context.Add(archTasks);
                _context.SaveChanges();
                projects.Tasks.Add(archTasks);
                projects.ArchiveTaskId = archTasks.TaskId;
                subtask.ArchiveTaskId = archTasks.TaskId;


                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = projects.ProjectId });
            }
            catch
            {
                return NotFound();
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

        [HttpPost]
        public async Task<IActionResult> EditName(int projId, string projName)
        {
            try
            {
                var projects = _context.Projects.Where(p => p.ProjectId == projId).FirstOrDefault();
                projects.ProjName = projName;
                _context.Update(projects);
                await _context.SaveChangesAsync();
                return PartialView();
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProjectId,ProjName,ProjImagePath,Archive")] Projects projects)
        {
            try
            {
                _context.Update(projects);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = projects.ProjectId }); //if Archive stay true => redirect to all projects
            }
            catch
            {
                return NotFound();
            }
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




        string[] scopes = { DriveService.Scope.Drive };
        string appName = "GeekOn";


        public void GetPath()
        {
            UserCredential credential;
            credential = GetCredentials();

            //create Service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });

            CreateFolder("folderrrrrrrrr", service);

        }

        private void CreateFolder(string folderName, DriveService service)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();

        }

        public UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {

                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quikstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return credential;

        }




    }
}
