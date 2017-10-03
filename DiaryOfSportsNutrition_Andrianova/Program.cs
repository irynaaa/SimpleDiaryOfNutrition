using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DiaryOfSportsNutrition_Andrianova
{
    class Program
    {
        static void Print(IEnumerable<Food> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
                
            }
            Console.WriteLine();
        }

        static void showProdInPlate(DateTime t)
        {
            using (DiaryNutrionContext db = new DiaryNutrionContext())
            {
                var ac = db.MyPlates;
                var prod = db.Products;
                var pfr = db.PlateFoodRecords;

                //var ShowProducts = from b in db.MyPlates
                //                   join prf in db.PlateFoodRecords on b.Id equals prf.PlateId
                //                   join p in db.Products on prf.FoodId equals p.Id
                //                   where b.mealtime == t
                //                   select new
                //                   { time = b.mealtime,
                //                     food=p.FoodName,
                //                     proteins=p.Proteins * prf.Weight/100,
                //                     fats=p.Fat * prf.Weight / 100,
                //                     carbohydrates =p.Carbohydrates * prf.Weight / 100,
                //                     call =p.CaloricValue*prf.Weight/100
                //                   };

                var ShowProducts = from b in db.PlateFoodRecords
                                   join mpl in db.MyPlates on b.PlateId equals mpl.Id
                                   join p in db.Products on b.FoodId equals p.Id
                                   where mpl.mealtime == t
                                   select new
                                   {
                                       time = mpl.mealtime,
                                       food = p.FoodName,
                                       proteins = p.Proteins * b.Weight / 100,
                                       fats = p.Fat * b.Weight / 100,
                                       carbohydrates = p.Carbohydrates * b.Weight / 100,
                                       call = p.CaloricValue * b.Weight / 100
                                   };

                foreach (var p in ShowProducts)
                {
                    Console.WriteLine($"{p.time} Состав: {p.food}  Белки: {p.proteins} Жиры: {p.fats} Углеводы: {p.carbohydrates} Калории: {p.call}");
                }
            }
        }
        static void Main(string[] args)
        {
            //using (DiaryOfSportsNutrition_Andrianova.DiaryNutrionContext db = new DiaryOfSportsNutrition_Andrianova.DiaryNutrionContext())
            //{
            //    var ac = db.Products;
            //    foreach (var a in ac)
            //    {
            //        Console.WriteLine(a.Id + ". " + a.FoodName + " Состав: Белки: " + a.Proteins + " Жиры: " + a.Fat + " Углеводы: " + a.Carbohydrates);
            //    }
            //    Console.WriteLine();

            //    var mp = db.MyPlates;
            //    foreach (var mpl in mp)
            //    {
            //        showProdInPlate(mpl.mealtime);
            //        Console.WriteLine("________________________________________________________-");
            //    }
            //    Console.WriteLine();




                //var t = db.MealTimes;
                //  foreach (MealTime m in db.MealTimes.Include(m=>m.ProductsInPlate))
                // {
                //       Console.WriteLine("Прием пищи: " + m.mealtime);
                //foreach (MyPlate p in m.ProductsInPlate)//.Where(x=>m.Id==x.Id))
                //{
                //    Console.WriteLine($"FoodId {p.FoodId}");
                //}


                //  showProdInPlate(m.mealtime);
                // }


               // }
            }
    }
}
