using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Project_WEB2.Models;
/*
 * API 
 */
namespace Project_WEB2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getnameAPI : ControllerBase
    {

        [HttpGet("{ro}")]
        public IEnumerable<GetName> Get(string ro)
        {
            List<GetName> li = new List<GetName>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Project_WEB2Context");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM userall where role ='" + ro + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                li.Add(new GetName
                {
                    name = (string)reader["name"],
                });

            }

            reader.Close();
            conn1.Close();
            return li;
        }
    }
}
