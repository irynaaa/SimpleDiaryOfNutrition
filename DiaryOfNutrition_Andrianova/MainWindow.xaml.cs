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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiaryOfSportsNutrition_Andrianova;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using Autofac;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Concrete;

namespace DiaryOfNutrition_Andrianova
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
   
   public partial class MainWindow : Window
    {
        private static IContainer Container { get; set; }

        public IList<MyPlate> plates = new List<MyPlate>();
        public int plateID = 1;
        public bool new_plate = false;
        List<PlateItems> items = new List<PlateItems>();

        public string SourceUri
        {
            get { return System.IO.Path.GetFullPath("Resources/icon.png"); }
        }
        
        public MainWindow()
        {
            InitializeComponent();

            ProductComboBox.ItemsSource = GetData();
            MealTimePicker.SelectedDate = DateTime.Now;

        }

        private ArrayList GetData()
        {
            ArrayList data = new ArrayList();

            var builder = new ContainerBuilder();
            builder.RegisterType<EFContext>().As<IEFContext>();
            builder.RegisterType<FoodRepository>().As<IFoodRepository>();
            Container = builder.Build();

            IFoodRepository foodRep = Container.Resolve<IFoodRepository>();
            foreach (var u in foodRep.Products())
            {
                data.Add(u);
            }
            return data;
        }

       

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Food f = new Food();
            ComboBox cb = (ComboBox)sender;
            try
            {
                if (cb.SelectedValue.ToString() == null) return;
            }
            catch(NullReferenceException)
            {
                return;
            }
            f = SelectedItem(ProductComboBox.SelectedValue.ToString());
            FatsTextBox.Text = f.Fat.ToString();
            ProteinsTextBox.Text = f.Proteins.ToString();
            CarbohydratesTextBox.Text = f.Carbohydrates.ToString();
            CaloriesTextBox.Text = f.CaloricValue.ToString();
        }

        static Food SelectedItem(string foodName)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EFContext>().As<IEFContext>();
            builder.RegisterType<FoodRepository>().As<IFoodRepository>();
            
            Container = builder.Build();

            IFoodRepository foodRep = Container.Resolve<IFoodRepository>();
            if (foodName == null) return null;
            
                Food food = new Food();
                var prod = foodRep.Products();
                var result = from c in prod
                             where c.FoodName == foodName
                             select c;
                foreach (var f in result)
                {
                    food.Id = f.Id;
                    food.FoodName=f.FoodName;
                    food.Proteins = f.Proteins;
                    food.Fat = f.Fat;
                    food.Carbohydrates = f.Carbohydrates;
                    food.CaloricValue = f.CaloricValue;
                }
                return food;
        }

       

        private void WeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void buttonAddDish_Click(object sender, RoutedEventArgs e)
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

            Food f = new Food();
            MyPlate mp = new MyPlate();
            if (ProductComboBox.SelectedItem!=null)
            {
                f = SelectedItem(ProductComboBox.SelectedValue.ToString());
            }
            else
            {
                MessageBox.Show("Блюдо должно быть указано обязательно!");
                return;
            }

            if (f == null || String.IsNullOrEmpty(WeightTextBox.Text) == true)
            {
                MessageBox.Show("Блюдо и его вес должны быть указаны обязательно!");
                return;
            }
                    int h = 0;
                    int m = 0;
                    int s = 0;
            try
            {
                h = Int32.Parse(comboBoxHour.SelectedValue.ToString().Split(':')[1]);
                m = Int32.Parse(comboBoxMinutes.SelectedValue.ToString().Split(':')[1]);
                mp = new MyPlate
                {
                    mealtime = new DateTime(MealTimePicker.SelectedDate.Value.Year, MealTimePicker.SelectedDate.Value.Month, MealTimePicker.SelectedDate.Value.Day, h, m, s),
                };
                myplateRep.Add(mp);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Не указано время!");
                return;
            }

            platefoodrecordRepositoryRep.Add(new PlateFoodRecord { FoodId = f.Id, PlateId = mp.Id, Weight = float.Parse(WeightTextBox.Text) });
            MessageBox.Show("Блюдо в тарелке!");
        }

        private void MealTimePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void ProductComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            cb.IsEditable = true;
            
            try { 
           if(cb.SelectedValue.ToString()==null||SelectedItem(cb.SelectedValue.ToString())==null)
            {
                cb.IsEditable = false;
                cb.SelectedItem = "";
                ProductComboBox.SelectedIndex = -1;
                return;
            }
            }
            catch(NullReferenceException)
            {
                return;
            }
        }

        private void buttonShowPlate_Click(object sender, RoutedEventArgs e)
        {
            ListPlatesInfo lpiWindow = new ListPlatesInfo();
            lpiWindow.Show();
        }

        private void ShowCurrentDayPlatebutton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = MealTimePicker.SelectedDate.Value;
            int m = t.Month;
            int y = t.Year;
            int d = t.Day;
            SqlDateTime day;
           
            day = new SqlDateTime(y, m, d, 0, 0, 0);

            CurrentPlateInfoWindow cpiW = new CurrentPlateInfoWindow(day); 
            cpiW.Show();
        }

        private void ShowRecommendForTodaybutton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = MealTimePicker.SelectedDate.Value;
            int m = t.Month;
            int y = t.Year;
            int d = t.Day;
            SqlDateTime day;
            day = new SqlDateTime(y, m, d, 0, 0, 0);
            DetalesAndRecommendationsWindow drecomW = new DetalesAndRecommendationsWindow(day);
            drecomW.Show();
        }

        private void ShowBalancebutton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = MealTimePicker.SelectedDate.Value;

            int m = t.Month;
            int y = t.Year;
            int d = t.Day;

            SqlDateTime day;

            day = new SqlDateTime(y, m, d, 0, 0, 0);
            FoodBalanceWindow drecomW = new FoodBalanceWindow (day);
            drecomW.Show();
        }

        private void statistic_btn_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = MealTimePicker.SelectedDate.Value;

            int m = t.Month;
            int y = t.Year;
            int d = t.Day;

            SqlDateTime day;
            SqlDateTime week;
            SqlDateTime month;

            day = new SqlDateTime(y, m, d, 0, 0, 0);

            DateTime dateForStatWeek = DateTime.Now.AddDays(-7);

            DateTime dateForStatMonth = DateTime.Now.AddDays(-30);

            week = dateForStatWeek;
            month = dateForStatMonth;
            Statistic statW = new Statistic(week, month);
            statW.Show();
        }
    }
}
