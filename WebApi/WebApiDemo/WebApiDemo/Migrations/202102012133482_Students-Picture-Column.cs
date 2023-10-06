namespace WebApiDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentsPictureColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Photo", c => c.Binary(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Photo");
        }
    }
}
