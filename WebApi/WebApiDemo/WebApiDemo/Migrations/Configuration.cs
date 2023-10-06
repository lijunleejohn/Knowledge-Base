namespace WebApiDemo.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApiDemo.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApiDemo.Models.SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApiDemo.Models.SchoolContext context)
        {
            //  This method will be called after migrating to the latest version.
            if (!context.Genders.Any())
            {
                context.Genders.SeedEnumValues<Gender, GenderEnum>(@enum => @enum);
                context.SaveChanges();
            }
            base.Seed(context);
        }
    }
}
