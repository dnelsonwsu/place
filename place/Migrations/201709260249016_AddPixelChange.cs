namespace place.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPixelChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PixelChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Color = c.String(),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        CanvasVersion = c.Int(nullable: false),
                        Canvas_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Canvas", t => t.Canvas_Name)
                .Index(t => t.Canvas_Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PixelChanges", "Canvas_Name", "dbo.Canvas");
            DropIndex("dbo.PixelChanges", new[] { "Canvas_Name" });
            DropTable("dbo.PixelChanges");
        }
    }
}
