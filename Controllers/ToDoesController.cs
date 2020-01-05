using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ToDoes
        public IActionResult Index()
        {
            return View();
        }

        private IEnumerable<ToDo> GetMyToDoes()
        {
            string currentUserId = User.Identity.GetUserId();
            IdentityUser currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);

            IEnumerable<ToDo> myToDoes = _context.ToDos.Where(x => x.User == currentUser).ToList();

            int completed = myToDoes.Where(x => x.IsDone == true).Count();

            ViewBag.Percent = Math.Round(100f * ((float)completed / (float)myToDoes.Count()));
          
            return myToDoes;

        }

        public IActionResult BuildToDoTable()
        {

            return PartialView("_ToDoTable", GetMyToDoes());
        }

        // GET: ToDoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // GET: ToDoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {

                string currentUserId = User.Identity.GetUserId();
                IdentityUser currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId );
                toDo.User = currentUser;
                _context.Add(toDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjaxCreate([Bind("Id,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {

                string currentUserId = User.Identity.GetUserId();
                IdentityUser currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
                toDo.User = currentUser;
                toDo.IsDone = false;
                _context.Add(toDo);
                await _context.SaveChangesAsync();
               
            }
            return PartialView("_ToDoTable",GetMyToDoes());
        }

        // GET: ToDoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return View(toDo);
        }

        [HttpPost]
        public IActionResult AjaxEdit(int? id, bool value)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = _context.ToDos.Find(id);
            if (toDo == null)
            {
                return NotFound();
            }
            else
            {
                toDo.IsDone = value;
                _context.Update(toDo);
                _context.SaveChanges();
                return PartialView("_ToDoTable", GetMyToDoes());
            }

            
        }
        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,IsDone")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
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
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }
                     
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
      
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDo = await _context.ToDos.FindAsync(id);
            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();
            return PartialView("_ToDoTable", GetMyToDoes());
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.Id == id);
        }
    }
}
