using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApiDemo.Models
{
    public class SchoolContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public SchoolContext() : base("SchoolDBConnectionString")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    }
}