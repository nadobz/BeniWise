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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DAN
            optionsBuilder.UseSqlServer(
            "Server=OSI-PC173\\SQLEXPRESS;"
            + "Database=BeniWise;"
            + "Integrated Security=true;"
            + "TrustServerCertificate=true;");
            // ALGY
            //  optionsBuilder.UseSqlServer(
            //  "Server=localhost\\SQLEXPRESS;"
            //  + "Database=BeniWise;"
            //  + "Integrated Security=true;"
            //  + "TrustServerCertificate=true;");
            // DAPHNE
            //   optionsBuilder.UseSqlServer(
            //   "Server=localhost\\SQLEXPRESS;"
            //   + "Database=BeniWise;"
            //   + "Integrated Security=true;"
            //   + "TrustServerCertificate=true;");


        }
    }
}
