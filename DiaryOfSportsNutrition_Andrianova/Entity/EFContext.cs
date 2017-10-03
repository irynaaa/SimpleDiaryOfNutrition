using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;

namespace DiaryOfSportsNutrition_Andrianova.Entity
{
    public class EFContext : DbContext, IEFContext
    {
        public EFContext() : base("ExamDiaryDB_AndrianovaConnectionString")
        {
          //  Database.SetInitializer<EFContext>(null);
        }
        public EFContext(string conName) : base(conName)
        {
            Database.SetInitializer<EFContext>(null);
        }
        public DbSet<Food> Products { get; set; }
        public DbSet<MyPlate> MyPlates { get; set; }
        public DbSet<PlateFoodRecord> PlateFoodRecords { get; set; }

        IDbSet<TEntity> IEFContext.Set<TEntity>()
        {
            return base.Set<TEntity>();
        }


    }
}
