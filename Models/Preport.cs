using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Project_WEB2.Models
{
    public class Preport
    {
        public int Id { get; set; }
        public string customername { get; set; }
        public int total { get; set; }


    }
}
