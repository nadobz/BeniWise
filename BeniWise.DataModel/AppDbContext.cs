using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeniWise.DataModel
{
    public class AppDbContext : DbContext
    {

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;

    }
}
