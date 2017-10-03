using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DiaryOfSportsNutrition_Andrianova
{
   public class DiaryNutrionContext: DbContext
    {
        public DiaryNutrionContext() : base("ExamDiaryDB_AndrianovaConnectionString")
        {
            //Database.SetInitializer<DiaryNutrionContext>(new MyInitializer());
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiaryNutrionContext, 
           //     DiaryOfSportsNutrition_Andrianova.Migrations.Configuration>("ExamDiaryDB_AndrianovaConnectionString"));
        }
        public DbSet<Food> Products { get; set; }
        public DbSet<MyPlate> MyPlates { get; set; }
        public DbSet<PlateFoodRecord> PlateFoodRecords { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
