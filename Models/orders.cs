using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Project_WEB2.Models
{
    public class orders
    {
        public int id { get; set; }
        public int itemid { get; set; }
        public int userid { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buydate { get; set; }
        public int quantity { get; set; }

    }
}
