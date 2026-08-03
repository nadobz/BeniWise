using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeniWise.DataModel
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DAN
               optionsBuilder.UseSqlServer(
               "Server=OSI-PC173\\SQLEXPRESS;"
               + "Database=BeniWise;"
               + "Integrated Security=true;"
               + "TrustServerCertificate=true;");
            // ALGY
            //optionsBuilder.UseSqlServer(
               //"Server=localhost\\SQLEXPRESS;"
               //+ "Database=BeniWise;"
              // + "Integrated Security=true;"
              // + "TrustServerCertificate=true;");
            // DAPHNE


        }
    }
}
