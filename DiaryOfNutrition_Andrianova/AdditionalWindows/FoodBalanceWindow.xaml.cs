﻿using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;
using DiaryOfSportsNutrition_Andrianova;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Логика взаимодействия для FoodBalanceWindow.xaml
    /// </summary>
    public partial class FoodBalanceWindow : Window
    {
        private static IContainer Container { get; set; }
        List<DailyManuDetales> _items = new List<DailyManuDetales>();

        public FoodBalanceWindow(SqlDateTime t)
        {
            InitializeComponent();
            showProdInPlate_(t);
        }
        private void showProdInPlate_(SqlDateTime t)
        {
            float proteins = 0;
            float fats = 0;
            float carbohydrates = 0;
            float ccal = 0;
            float totalWeight = 0;

            var builder = new ContainerBuilder();
            builder.RegisterType<EFContext>().As<IEFContext>();
            builder.RegisterType<FoodRepository>().As<IFoodRepository>();
            builder.RegisterType<MyPlateRepository>().As<IMyPlateRepository>();
            builder.RegisterType<PlateFoodRecordRepository>().As<IPlateFoodRecordRepository>();
            Container = builder.Build();

            IFoodRepository foodRep = Container.Resolve<IFoodRepository>();
            IMyPlateRepository myplateRep = Container.Resolve<IMyPlateRepository>();
            IPlateFoodRecordRepository platefoodrecordRepositoryRep = Container.Resolve<IPlateFoodRecordRepository>();

            List<Food> products = new List<Food>();
            List<MyPlate> mypl = new List<MyPlate>();
            List<PlateFoodRecord> pfr = new List<PlateFoodRecord>();
            foreach (var u in foodRep.Products())
            {
                products.Add(u);
            }
            foreach (var u in myplateRep.MyPlates())
            {
                mypl.Add(u);
            }

            foreach (var u in platefoodrecordRepositoryRep.PlateFoodRecords())
            {
                pfr.Add(u);
            }

            var ShowProducts =
            from mpl in mypl
            join r in pfr on mpl.Id equals r.PlateId
            join p in products on r.FoodId equals p.Id
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