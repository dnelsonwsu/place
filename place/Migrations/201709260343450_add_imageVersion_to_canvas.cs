namespace place.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_imageVersion_to_canvas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Canvas", "ImageVersion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Canvas", "ImageVersion");
        }
    }
}
