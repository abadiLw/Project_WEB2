using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Project_WEB2.Models;

namespace Project_WEB2.Data
{
    public class Project_WEB2Context : DbContext
    {
        public Project_WEB2Context (DbContextOptions<Project_WEB2Context> options)
            : base(options)
        {
        }

        public DbSet<Project_WEB2.Models.items> items { get; set; } = default!;
        public DbSet<Project_WEB2.Models.orders>? orders { get; set; }
        public DbSet<Project_WEB2.Models.userall>? userall { get; set; }
        public DbSet<Project_WEB2.Models.orderdetail> orderdetail { get; set; }
        public DbSet<Project_WEB2.Models.Preport> Preport { get; set; }

    }
}
