using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;
using DiaryOfSportsNutrition_Andrianova;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;

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
           
            month = new SqlDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            comboBoxMonth.SelectedIndex = month.Value.Month-1;
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

            var ShowProducts =
            from mpl in myplateRep.MyPlates().AsEnumerable()
            join r in platefoodrecordRepositoryRep.PlateFoodRecords().AsEnumerable() on mpl.Id equals r.PlateId
            join p in foodRep.Products().AsEnumerable() on r.FoodId equals p.Id
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
           // CultureInfo cru = new CultureInfo("ru-RU");
            //CultureInfo cinv = CultureInfo.InvariantCulture;
            string strdata= "";
            CultureInfo ci = CultureInfo.InstalledUICulture;
            DateTime userdate = DateTime.Now;
            userdate = new DateTime(DateTime.Now.Year, (comboBoxMonth.SelectedIndex + 1),DateTime.Now.Day);
            strdata=userdate.ToString("d", ci);
            showProdInPlate(ConvertToDateTime(strdata));
        }
        private static DateTime ConvertToDateTime(string value)
        {
            DateTime convertedDate=DateTime.Now;
           

            try
            {
                convertedDate = Convert.ToDateTime(value);
            }
            catch (FormatException)
            {
            }
            return convertedDate;
        }
    }

}

