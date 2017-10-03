﻿using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;
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
using DiaryOfSportsNutrition_Andrianova;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Interaction logic for Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        private static IContainer Container { get; set; }
        private static SqlDateTime _day { get; set; }
        private static SqlDateTime _week { get; set; }
        private static SqlDateTime _month { get; set; }
        public Statistic(SqlDateTime week, SqlDateTime month)
        {
            InitializeComponent();
            _day = new SqlDateTime();
            _day = DateTime.Now;
            _week = new SqlDateTime();
            _week = week;
            _month = new SqlDateTime();
            _month = month;
        }

        private void statWeekBtn_Click(object sender, RoutedEventArgs e)
        {

            showStat_(_day, _week, 7);
        }

        private void showStat_(SqlDateTime d, SqlDateTime t, int days)
        {
            List<PlateItems> items_ = new List<PlateItems>();
            List<PlateItems> user_stat = new List<PlateItems>();
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
            where mpl.mealtime.Date >= t.Value.Date && mpl.mealtime.Date <= d.Value.Date
            select new
            {
                time = mpl.mealtime,
                food = p.FoodName,
                proteins = p.Proteins * r.Weight / 100,
                fats = p.Fat * r.Weight / 100,
                carbohydrates = p.Carbohydrates * r.Weight / 100,
                call = p.CaloricValue * r.Weight / 100
            };
            foreach (var p in ShowProducts.OrderBy(x => x.time.Hour))//отсортировала время по возрастанию
            {
                items_.Add(new PlateItems() { _time = p.time, _food = p.food, _proteins = p.proteins, _fats = p.fats, _carbohydrates = p.carbohydrates, _call = p.call });
            }

            var most = (from i in items_
                        group i._food by i._food into grp
                        orderby grp.Count() descending
                        select new { grp.Key, Cnt = grp.Count() }).Where(r => r.Cnt > 1);

            foreach (var p in most)
            {
                user_stat.Add(new PlateItems() { _count = p.Cnt, _food = p.Key});
            }
            dataGridStatistic.ItemsSource = user_stat;

            float avg = 0;
            if (days == 7)
            {
                avg = items_.Sum(x => x._call)/7;
            }
            else
            { avg = items_.Sum(x => x._call)/30; }

            labelAvgCal.Content = avg.ToString("0.00")+" ccal";
        }

     
        private void statMonthBtn_Click(object sender, RoutedEventArgs e)
        {
            showStat_(_day, _month, 30);
        }
    }
}
