using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Project_WEB2.Data;
using Project_WEB2.Models;

namespace Project_WEB2.Controllers
{
    public class itemsController : Controller
    {
        private readonly Project_WEB2Context _context;

        public itemsController(Project_WEB2Context context)
        {
            _context = context;
        }

        // index________________________________________________________________
        public async Task<IActionResult> Index()
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");

            return _context.items != null ? 
                          View(await _context.items.ToListAsync()) :
                          Problem("Entity set 'Project_WEB2Context.items'  is null.");
        }
        // _____________________________________________________________________
        // Details______________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }
        // _____________________________________________________________________
        // Create_______________________________________________________________
        public IActionResult Create()
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "userall");
            }
        }
        //----
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,name,descr,price,quantity,discount,category")] items items)
        {
            {
                if (file != null)
                {
                    string filename = file.FileName; 
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    items.imagefilename = filename;
                }

                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        // _____________________________________________________________________
        // Edit_________________________________________________________________
        public async Task<IActionResult> Edit(int? id)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }
        //----

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename")] items items)
        {
            if (id != items.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!itemsExists(items.Id))
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
            return View(items);
        }
        // _____________________________________________________________________
        // Delete_______________________________________________________________
        public async Task<IActionResult> Delete(int? id)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }

            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }
        //-------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.items == null)
            {
                return Problem("Entity set 'Project_WEB2Context.items'  is null.");
            }
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // _____________________________________________________________________
        // itemsExists__________________________________________________________
        private bool itemsExists(int id)
        {
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        // _____________________________________________________________________
        // imageslider__________________________________________________________
        public async Task <IActionResult> imageslider()
        {
            return View(await _context.items.ToListAsync());
        }
        // _____________________________________________________________________
        // itemslist____________________________________________________________
        public async Task<IActionResult> itemslist()
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }

            var orItems = await _context.items.FromSqlRaw("SELECT * FROM items order by category ").ToListAsync();

            return View(orItems);
        }
        // _____________________________________________________________________
        // dashboard____________________________________________________________
        public async Task<IActionResult> dashboard()
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            string sql = "";

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn = new SqlConnection(conStr);
            conn.Open();
            SqlCommand comm;

            
                sql = "SELECT COUNT( Id)  FROM items where category =1";
                comm = new SqlCommand(sql, conn);
                ViewData["d1"] = (int)comm.ExecuteScalar();

                sql = "SELECT COUNT( Id)  FROM items where category =2";
                comm = new SqlCommand(sql, conn);
                ViewData["d2"] = (int)comm.ExecuteScalar();

                sql = "SELECT sum(quantity)  FROM orders";
                comm = new SqlCommand(sql, conn);
                ViewData["d3"] = (int)comm.ExecuteScalar();

                sql = "SELECT COUNT( Id)  FROM items where category =3";
                comm = new SqlCommand(sql, conn);
                ViewData["d4"] = (int)comm.ExecuteScalar();

                sql = "SELECT COUNT( Id)  FROM items where category =4";
                comm = new SqlCommand(sql, conn);
                ViewData["d5"] = (int)comm.ExecuteScalar();


            conn.Close();
            return View();

        }

    }
}
