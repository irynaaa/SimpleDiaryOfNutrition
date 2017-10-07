using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiaryOfSportsNutrition_Andrianova;
using System.Collections;
using System.Data.SqlTypes;
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
        List<Food> foodlist = new List<Food>();
        public string SourceUri
        {
            get { return System.IO.Path.GetFullPath("Resources/icon.png"); }
        }
        
        public MainWindow()
        {
            InitializeComponent();

            ProductComboBox.ItemsSource = GetData();
            List<Food> list = GetData();
            foreach (var p in list)
            { foodlist.Add(p); }
            MealTimePicker.SelectedDate = DateTime.Now;

        }

        

        private List<Food> GetData()
        {
            List<Food> data = new List<Food>();

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

        private Food SelectedItem(string foodName)
        {

            if (foodName == null) return null;

            Food food = new Food();

            food = foodlist.Find(p => p.FoodName.Contains(foodName));
            return food;
        }



        private void buttonAddDish_Click(object sender, RoutedEventArgs e)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EFContext>().As<IEFContext>();
            builder.RegisterType<MyPlateRepository>().As<IMyPlateRepository>();
            builder.RegisterType<PlateFoodRecordRepository>().As<IPlateFoodRecordRepository>();
            Container = builder.Build();

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
            else
            {
                int parsed = -1;
                if (int.TryParse(WeightTextBox.Text, out parsed) == true)
                {

                }
                else
                {
                    MessageBox.Show("Некорректно введены данные!");
                    return;
                }
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

      

        private void ProductComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                try { 
                    foodlist.Where(p => p.FoodName.Contains(ProductComboBox.SelectedValue.ToString())).ToList();
                    WeightTextBox.Focus();
                    e.Handled = true;
                    return;
                }
                catch(NullReferenceException)
                {
                    MessageBox.Show("Продукт не найден!");
                    ProductComboBox.SelectedValue = null;
                    e.Handled = true;
                    return;
                }
                }
            else
            {
                return;
            }
        }

    }
}
