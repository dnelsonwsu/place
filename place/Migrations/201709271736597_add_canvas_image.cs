namespace place.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_canvas_image : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CanvasImages",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Image = c.Binary(),
                        canvas_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Name)
                .ForeignKey("dbo.Canvas", t => t.canvas_Name)
                .Index(t => t.canvas_Name);
            
            DropColumn("dbo.Canvas", "Image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Canvas", "Image", c => c.Binary());
            DropForeignKey("dbo.CanvasImages", "canvas_Name", "dbo.Canvas");
            DropIndex("dbo.CanvasImages", new[] { "canvas_Name" });
            DropTable("dbo.CanvasImages");
        }
    }
}
