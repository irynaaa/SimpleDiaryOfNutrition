﻿using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;
using DiaryOfSportsNutrition_Andrianova;
using System;
using System.Collections.Generic;
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
using System.Data;
using System.Globalization;
using System.Data.SqlTypes;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Логика взаимодействия для ListPlatesInfo.xaml
    /// </summary>


    public partial class ListPlatesInfo : Window
    {
        private static SqlDateTime month;
        private static IContainer Container { get; set; }
        public ListPlatesInfo()
        {
            InitializeComponent();
            int mnth = DateTime.Parse("1." + DateTime.Now.Month + " "+DateTime.Now.Year).Month;
            comboBoxMonth.SelectedIndex = mnth-1;
            month = new SqlDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            showProdInPlate(month);
        }

        private void showProdInPlate(SqlDateTime t)
        {
            List<PlateItems> items_ = new List<PlateItems>();
            
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
            where mpl.mealtime.Month == t.Value.Month
            select new
            {
                time = mpl.mealtime,
                food = p.FoodName,
                proteins = p.Proteins * r.Weight / 100,
                fats = p.Fat * r.Weight / 100,
                carbohydrates = p.Carbohydrates * r.Weight / 100,
                call = p.CaloricValue * r.Weight / 100
            };

            foreach (var p in ShowProducts.OrderBy(x => x.time.Day).ThenBy(x => x.time.Hour))//отсортировала время по возрастанию
            {
                items_.Add(new PlateItems() { _time =p.time, _food = p.food, _proteins = p.proteins, _fats = p.fats, _carbohydrates = p.carbohydrates, _call = p.call });
                
            }

                dataGridAllPlates.ItemsSource = items_;
        }

        private void comboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _month = DateTime.Parse("1." + comboBoxMonth.SelectedValue.ToString().Split(':')[1] +" " + DateTime.Now.Year).Month;
            month = new SqlDateTime(DateTime.Now.Year, _month, DateTime.Now.Day);
            showProdInPlate(month);
        }
    }

}
