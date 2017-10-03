namespace DiaryOfSportsNutrition_Andrianova.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyPlates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        mealtime = c.DateTime(nullable: false),
                        PlateFoodRecord_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlateFoodRecords", t => t.PlateFoodRecord_Id)
                .Index(t => t.PlateFoodRecord_Id);
            
            CreateTable(
                "dbo.PlateFoodRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FoodId = c.Int(nullable: false),
                        PlateId = c.Int(nullable: false),
                        Weight = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Foods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FoodName = c.String(),
                        Proteins = c.Single(nullable: false),
                        Fat = c.Single(nullable: false),
                        Carbohydrates = c.Single(nullable: false),
                        CaloricValue = c.Single(nullable: false),
                        PlateId = c.Int(nullable: false),
                        PlateFoodRecord_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlateFoodRecords", t => t.PlateFoodRecord_Id)
                .Index(t => t.PlateFoodRecord_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MyPlates", "PlateFoodRecord_Id", "dbo.PlateFoodRecords");
            DropForeignKey("dbo.Foods", "PlateFoodRecord_Id", "dbo.PlateFoodRecords");
            DropIndex("dbo.Foods", new[] { "PlateFoodRecord_Id" });
            DropIndex("dbo.MyPlates", new[] { "PlateFoodRecord_Id" });
            DropTable("dbo.Foods");
            DropTable("dbo.PlateFoodRecords");
            DropTable("dbo.MyPlates");
        }
    }
}
