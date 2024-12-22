using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Project_WEB2.Models
{
    public class orderdetail
    {
        public int Id { get; set; }
        public string name { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buydate { get; set; }
        public int quantity { get; set; }
        public int totalprice { get; set; }

    }
}
