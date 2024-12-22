using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_WEB2.Data;
using Project_WEB2.Models;

namespace Project_WEB2.Controllers
{
    public class userallController : Controller
    {
        private readonly Project_WEB2Context _context;

        public userallController(Project_WEB2Context context)
        {
            _context = context;
        }

        // index________________________________________________________________
        public async Task<IActionResult> Index()
        {
            
            var r = HttpContext.Session.GetString("Role");
            if (r == "admin")
            {

                return _context.userall != null ?
                            View(await _context.userall.ToListAsync()) :
                            Problem("Entity set 'Project_WEB2Context.userall'  is null.");
            }
            else
            {
                return RedirectToAction(nameof(login));
            }
        }

        // _____________________________________________________________________
        // Details______________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userall == null)
            {
                return NotFound();
            }

            return View(userall);
        }

        // _____________________________________________________________________
        // Create_______________________________________________________________
        public IActionResult Create()
        {
            return View();
        }
        //______

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,password,role,RegistDate")] userall userall)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            if (ModelState.IsValid)
            {
                _context.Add(userall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userall);
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
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall.FindAsync(id);
            if (userall == null)
            {
                return NotFound();
            }
            return View(userall);
        }

        //____
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,password,role,RegistDate")] userall userall)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            if (id != userall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!userallExists(userall.Id))
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
            return View(userall);
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
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userall == null)
            {
                return NotFound();
            }

            return View(userall);
        }

        // _____________________________________________________________________
        // DeleteConfirmed______________________________________________________
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            if (_context.userall == null)
            {
                return Problem("Entity set 'Project_WEB2Context.userall'  is null.");
            }
            var userall = await _context.userall.FindAsync(id);
            if (userall != null)
            {
                _context.userall.Remove(userall);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // _____________________________________________________________________
        // userallExists________________________________________________________
        private bool userallExists(int id)
        {

            return (_context.userall?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        // _____________________________________________________________________
        // login________________________________________________________________
        public IActionResult login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                string usid = HttpContext.Request.Cookies["id"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);
                HttpContext.Session.SetString("userid", usid);

                return View();
            }
        }
        //_______
        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa, string auto)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql = "SELECT * FROM userall where name ='" + na + "' and  password ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string na1 = (string)reader["name"];
                string ro = (string)reader["role"];
                int id = (int)reader["id"];
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);
                HttpContext.Session.SetInt32("userid",id);

                reader.Close();
                conn1.Close();

                if (auto == "true")
                {
                    HttpContext.Response.Cookies.Append("Name", na1);
                    HttpContext.Response.Cookies.Append("Role", ro);      
                }


                if (ro == "admin")
                {
                    return RedirectToAction("adminHome", "userall");
                }
                else 
                {
                    return RedirectToAction("customerHome", "userall");
                }

            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }

        // _____________________________________________________________________
        // logout_______________________________________________________________
        public IActionResult logout()
        {

            return RedirectToAction(nameof(login));

        }
        // _____________________________________________________________________
        // customerHome_________________________________________________________
        public async Task <IActionResult> customerHome()
        {

            ViewBag.name = HttpContext.Session.GetString("Name");
            return View(await _context.items.ToListAsync());
        }
        // _____________________________________________________________________
        // adminHome____________________________________________________________
        public IActionResult adminHome() {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            ViewBag.name = HttpContext.Session.GetString("Name");
            return View();
        }

        // _____________________________________________________________________
        // registration_________________________________________________________
        public IActionResult registration()
        {

            return View();
        }
        //_____
        [HttpPost]
        public IActionResult registration(string name,string password)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn = new SqlConnection(conStr);
            conn.Open();
            string sql;
            Boolean flage = false;
            sql = "select * from userall where name = '" + name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {

                flage = true;
            }
            reader.Close();
            if (flage == true)
            {
                ViewData["error"] = "name already exists";
                conn.Close();
                return View();
            }
            else
            {
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", "customer");
                sql = "insert into userall (name,password,Registdate,role) values ('" + name + "','" + password + "',CURRENT_TIMESTAMP,'customer')"; 
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                sql = "select id from userall where name = '" + name + "'";
                comm = new SqlCommand(sql, conn);
                int userId = (int)comm.ExecuteScalar();
                HttpContext.Session.SetInt32("userid", userId);

                return RedirectToAction(nameof(customerHome));
            }                            
        }
        // _____________________________________________________________________
        // email________________________________________________________________
        public IActionResult email()
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            return View();
        }

        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("amrhaleem99@gmail.com");
            mail.To.Add(address); 
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("ssadiqq2016@gmail.com", "hewy hpsl rzcs tbou");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();

        }
        // _____________________________________________________________________
        // email____________________________________________________________
        public async Task<IActionResult> Searchall()
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            userall brItem = new userall();
                return View();         
        }
        
        [HttpPost]

        public async Task<IActionResult> Searchall(string tit)
        {
            var bkItems = await _context.userall.FromSqlRaw("select * from userall where name = '" + tit + "' ").FirstOrDefaultAsync();

            return View(bkItems);
        }
        // _____________________________________________________________________
        // addadmin_____________________________________________________________
        public async Task<IActionResult> addadmin()
        {

            return View();
        }
        //______
        [HttpPost]
        public IActionResult addadmin(string adminname, string password, string cpassword)
        {
            var r = HttpContext.Session.GetString("Role");
            if (r == "customer")
            {
                return RedirectToAction("login", "userall");
            }
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn = new SqlConnection(conStr);
            conn.Open();
            string sql;
            Boolean flage = false;
            sql = "select * from userall where name = '" + adminname + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                flage = true;
            }
            reader.Close();

            if (password != cpassword)
            {
                ViewData["message"] = "Passwords Are Not Matched";
                return View();
            }
            if (flage == true)
            {
                ViewData["message"] = "Wrong..Admin Already Exists";
                conn.Close();
                return View();
            }
            else
            {
                HttpContext.Session.SetString("Name", adminname);
                HttpContext.Session.SetString("Role", "admin");
                sql = "insert into userall (name,password,Registdate,role) values ('" + adminname + "','" + password + "',CURRENT_TIMESTAMP,'admin')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                sql = "select id from userall where name = '" + adminname + "'";
                comm = new SqlCommand(sql, conn);
                int userId = (int)comm.ExecuteScalar();
                HttpContext.Session.SetString("userid", Convert.ToString(userId));

                return View();

            }


        }


    }
}

