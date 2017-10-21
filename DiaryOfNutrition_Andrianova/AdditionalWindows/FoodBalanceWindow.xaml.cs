using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;
using DiaryOfSportsNutrition_Andrianova;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Логика взаимодействия для FoodBalanceWindow.xaml
    /// </summary>
    public partial class FoodBalanceWindow : Window
    {
        private static IContainer Container { get; set; }

        public IFoodRepository foodRep;
        public IMyPlateRepository myplateRep;
        public IPlateFoodRecordRepository platefoodrecordRepositoryRep;
        public ContainerBuilder builder;

        List<DailyManuDetales> _items = new List<DailyManuDetales>();

        public FoodBalanceWindow(SqlDateTime t)
        {
            InitializeComponent();

            builder = new ContainerBuilder();
            builder.RegisterModule(new DataModule("ExamDiaryDB_AndrianovaConnectionString"));

            builder.RegisterType<EFContext>().As<IEFContext>();
            builder.RegisterType<FoodRepository>().As<IFoodRepository>();
            builder.RegisterType<MyPlateRepository>().As<IMyPlateRepository>();
            builder.RegisterType<PlateFoodRecordRepository>().As<IPlateFoodRecordRepository>();
            Container = builder.Build();

            foodRep = Container.Resolve<IFoodRepository>();
            myplateRep = Container.Resolve<IMyPlateRepository>();
            platefoodrecordRepositoryRep = Container.Resolve<IPlateFoodRecordRepository>();

            showProdInPlate_(t);
        }
        private void showProdInPlate_(SqlDateTime t)
        {
            float proteins = 0;
            float fats = 0;
            float carbohydrates = 0;
            float ccal = 0;
            float totalWeight = 0;

            var ShowProducts =
            from mpl in myplateRep.MyPlates().AsEnumerable()
            join r in platefoodrecordRepositoryRep.PlateFoodRecords().AsEnumerable() on mpl.Id equals r.PlateId
            join p in foodRep.Products().AsEnumerable() on r.FoodId equals p.Id
            where mpl.mealtime.Day == t.Value.Day && mpl.mealtime.Month == t.Value.Month
            select new
            {
                time = mpl.mealtime,
                food = p.FoodName,
                proteins = p.Proteins * r.Weight / 100,
                fats = p.Fat * r.Weight / 100,
                carbohydrates = p.Carbohydrates * r.Weight / 100,
                call = p.CaloricValue * r.Weight / 100
            };

            foreach (var p in ShowProducts)
                {
                    proteins += p.proteins;
                    fats += p.fats;
                    carbohydrates += p.carbohydrates;
                    ccal += p.call;
                }

            totalWeight = proteins + fats + carbohydrates;

            try
            {
                UserC.Value = (int)(carbohydrates*2*100/totalWeight);
                UserProteins.Value = (int)(proteins * 2 * 100 / totalWeight);
                UserFats.Value = (int)(fats * 2 * 100 / totalWeight);
                TBuserF.Text = (UserFats.Value/2).ToString();
                TBuserP.Text = (UserProteins.Value/2).ToString();
                TBuserC.Text = (UserC.Value / 2).ToString();
            }
            catch (Exception)
            {
                return;
            }

        }
    }

}
