using Autofac;
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
using System.Globalization;
using System.Collections.ObjectModel;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Логика взаимодействия для CurrentPlateInfoWindow.xaml
    /// </summary>
    public partial class CurrentPlateInfoWindow : Window
    {
        public List<PlateItems> items_ = new List<PlateItems>();
        private static IContainer Container { get; set; }
        public CurrentPlateInfoWindow(SqlDateTime t)
        {
            InitializeComponent();

            showProdInPlate_(t);

        }
        private void showProdInPlate_(SqlDateTime t)
        {
            
            dataGridCurrentPlate.Items.Clear();

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
           
            foreach (var p in ShowProducts.OrderBy(x => x.time.Hour))//отсортировала время по возрастанию
            {
                items_.Add(new PlateItems() { _time = p.time, _food = p.food, _proteins = p.proteins, _fats = p.fats, _carbohydrates = p.carbohydrates, _call = p.call });
            }

            dataGridCurrentPlate.ItemsSource = items_;
        }

        //remove the selectedItem from the collection source
        private void removeDish_btn_Click(object sender, RoutedEventArgs e)
        {
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

            if (dataGridCurrentPlate.SelectedIndex >= 0)
            {
                
                PlateItems myplate = dataGridCurrentPlate.SelectedItem as PlateItems;

                var ShowProducts =
                from mpl in mypl
                join r in pfr on mpl.Id equals r.PlateId
                join p in products on r.FoodId equals p.Id
                where
                mpl.mealtime.Year == myplate._time.Year &&
                mpl.mealtime.Month == myplate._time.Month &&
                mpl.mealtime.Day == myplate._time.Day &&
                mpl.mealtime.Hour == myplate._time.Hour &&
                mpl.mealtime.Minute == myplate._time.Minute &&
                p.FoodName==myplate._food
                select mpl;
               
                foreach (var p in ShowProducts)
                {
                    myplateRep.Remove(p);
                    items_.RemoveAt(dataGridCurrentPlate.SelectedIndex);
                    MessageBox.Show("Продукт удален!");
                }
                dataGridCurrentPlate.ItemsSource = null;
                dataGridCurrentPlate.ItemsSource = items_;
            }
        }
    }
}
