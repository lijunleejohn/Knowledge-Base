using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AzureMvcWithAuthenAuthor.Models
{
    public class IndustryDatabaseContext : DbContext
    {
        public IndustryDatabaseContext(DbContextOptions<IndustryDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<AzureMvcWithAuthenAuthor.Models.Product> Products { get; set; }
    }
}
