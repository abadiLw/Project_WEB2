using System.Composition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_WEB2.Data;
using Project_WEB2.Models;

namespace Project_WEB2.Controllers
{
    public class ordersController : Controller
    {
        private readonly Project_WEB2Context _context;

        public ordersController(Project_WEB2Context context)
        {
            _context = context;
        }

        public async Task <IActionResult> Purchase_page(int? id) {

           ViewData["role"] = HttpContext.Session.GetString("Role");
            var r = HttpContext.Session.GetString("Role");


            if (r == "admin")
            {
                var orItems = await _context.orderdetail.FromSqlRaw(
                    "select orders.id as id, items.name as name, orders.buydate as buydate, (items.price * orders.quantity) as totalprice, orders.quantity as quantity " +
                    "from items, orders, userall  where items.id = orders.itemid and orders.userid = userall.id and userall.id ='" + id + "' order by buydate  ").ToListAsync();
                return View(orItems);
            }
            else
            {
                int userid = Convert.ToInt32(HttpContext.Session.GetInt32("userid"));
                var orItems = await _context.orderdetail.FromSqlRaw(
                    "select orders.id as id, items.name as name, orders.buydate as buydate, (items.price * orders.quantity) as totalprice, orders.quantity as quantity " +
                    "from items, orders, userall  where items.id = orders.itemid and orders.userid = userall.id and userall.id ='" + userid + "' order by buydate  ").ToListAsync();
                return View(orItems);
            }

        }
        // _____________________________________________________________________
        // buy_page_____________________________________________________________
        public async Task<IActionResult> buy_page(int? id)
        {
            string ss = HttpContext.Session.GetString("Role");

            if (ss == "customer")
            {
                var car = await _context.items.FindAsync(id);
                return View(car);
            }
            else
                return RedirectToAction("login", "userall");
        }
        //________
        [HttpPost]
        public async Task<IActionResult> buy_page(int bookId, int quantity)
        {
            orders order = new orders(); 
            order.itemid = bookId;
            order.quantity = quantity;
            order.userid = Convert.ToInt32(HttpContext.Session.GetInt32("userid"));
            order.buydate = DateTime.Today;
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql;
            int qt = 0;
            sql = "select * from items where (id ='" + order.itemid + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                qt = (int)reader["quantity"];
            }
            reader.Close();
            conn.Close();
            if (order.quantity > qt)
            {
                ViewData["message"] = "maxiumam order quantity sould be " + qt;
                var book = await _context.items.FindAsync(bookId);
                return View(book);
            }
            else
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sql = "UPDATE items  SET quantity  = quantity   - '" + order.quantity + "'  where (id ='" + order.itemid + "' )";
                comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction(nameof(Purchase_page));
            }
        }
        // _____________________________________________________________________
        // purchase_report______________________________________________________
        public async Task<IActionResult> purchase_report()
        {

            var orItems = await _context.Preport.FromSqlRaw("select userall.id as Id, userall.name as customername, sum(orders.quantity*items.price) as total from items, orders, userall where  itemid = items.Id and userid = userall.Id group by userall.id, userall.name").ToListAsync();

            return View(orItems);
        }
    }
}