using Autofac;
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
    /// Логика взаимодействия для DetalesAndRecommendationsWindow.xaml
    /// </summary>
    public partial class DetalesAndRecommendationsWindow : Window
    {
        private static IContainer Container { get; set; }
        private static User user = new User();
        SqlDateTime _t;
        public DetalesAndRecommendationsWindow(SqlDateTime t)
        {

            InitializeComponent();
            _t = t;

            YourHeightTextBox.Text = user._Height.ToString();
            YourWeightTextBox.Text=user._Weight.ToString();
            YourAgeTextBox.Text = user._Age.ToString();
            YourCcal.Text = user._Ccal.ToString();
            if (user._Gender == true) radioButtonFemale.IsChecked = true;
            else radioButtonMale.IsChecked = true;

        }
        private void showProdInPlate_(SqlDateTime t)
        {
            List<DailyManuDetales> _items = new List<DailyManuDetales>();

            float proteins = 0;
            float fats = 0;
            float carbohydrates = 0;
            float ccal = 0;

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

            
            
            _items.Add(new DailyManuDetales { Title ="Белки", Completion= proteins, MinValue = 58, MaxValue =87, tooltip= "Рекоммендуемая суточная норма 58..87г. В вашем меню "+proteins.ToString()+" г", Color= "Green" });
            _items.Add(new DailyManuDetales { Title = "Жиры", Completion = fats, MinValue = 60, MaxValue = 102, tooltip = "Рекоммендуемая суточная норма 60..102г. В вашем меню " + fats.ToString() + " г", Color = "Green" });
            _items.Add(new DailyManuDetales { Title = "Углеводы", Completion = carbohydrates, MinValue = 257, MaxValue = 586, tooltip =  "Рекоммендуемая суточная норма 257..586г. В вашем меню " + carbohydrates.ToString() + " г", Color = "Green" });
            _items.Add(new DailyManuDetales { Title = "ССal", Completion = ccal, MinValue = 0, MaxValue = ((int)(user._Ccal) +500), tooltip =  "Рекоммендуемая суточная норма "+ YourCcal.Text + "ccal. В вашем меню " + ccal.ToString() + " ccal", Color = "Green" });
            if (proteins >= 58 && proteins <= 87)
            {
                _items[0].tooltip += "\nПотребление белков в пределах нормы!";
            }
            else if (proteins> 87)
            {
                _items[0].Color = "Red";
                _items[0].tooltip += "\nНеобходимо снизить потребление белков!";
            }
            else if (proteins <58)
            {
                _items[0].tooltip += "\nНеобходимо повысить потребление белков!";
            }



            if (fats >= 60&&fats <= 102)
            {
                _items[1].tooltip += "\nПотребление жиров в пределах нормы!";
               
            }
            else if(fats > 102)
            {
                _items[1].Color = "Red";
                _items[1].tooltip += "\nНеобходимо снизить потребление жиров!";
            }
            if (fats < 60 )
            {
                _items[1].tooltip += "\nНеобходимо повысить потребление жиров!";

            }

            if (carbohydrates >= 257&&carbohydrates<= 585)
            {
                _items[2].tooltip += "\nПотребление углеводов в пределах нормы!";
            }
            else if (carbohydrates > 585)
            {
                _items[2].Color = "Red";
                _items[2].tooltip += "\nНеобходимо снизить потребление углеводов!";
            }

            else if (carbohydrates<257)
            {
                _items[2].tooltip += "\nНеобходимо повысить потребление углеводов!";
            }


            if (ccal > (user._Ccal))
            {
                _items[3].Color = "Red";
                _items[3].tooltip += "\nВы рискуете попровиться!";
            }
            else
                _items[3].tooltip += "\nВаш рацион не добавит вам лишних килограммов!";

            DetalesOfDailyMenuList.ItemsSource = _items;
            DetalesOfDailyMenuList.Items.Refresh();

        }

  

        private void GetNormaCcal_Click(object sender, RoutedEventArgs e)
        {
            if (float.Parse(YourHeightTextBox.Text)==0|| float.Parse(YourWeightTextBox.Text)==0|| float.Parse(YourAgeTextBox.Text)==0)/*user._Age==0||user._Height==0 || user._Weight==0 || DetalesOfDailyMenuList.Visibility == Visibility.Collapsed)*/
            {
                MessageBox.Show("Введите данные!");

            }
            else 
            {
               
                try
                {
                    user._Height = float.Parse(YourHeightTextBox.Text);
                    user._Weight = float.Parse(YourWeightTextBox.Text);
                    user._Age = float.Parse(YourAgeTextBox.Text);
                    if (radioButtonFemale.IsChecked == true)
                    {
                        user._Gender = true;
                        user._Ccal = (float)447.6 + (9.2f * user._Weight) + (3.1f * user._Height) - (4.3f * user._Age);
                    }
                    else
                    {
                        user._Gender = false;
                        user._Ccal = (float)88.36 + (13.4f * user._Weight) + (4.8f * user._Height) - (5.7f * user._Age);
                    }
                    YourCcal.Text = user._Ccal.ToString();
                    showProdInPlate_(_t);
                    DetalesOfDailyMenuList.Visibility = Visibility.Visible;

                }
                catch (NullReferenceException)
                {
                    return;
                }
                catch (System.FormatException)
                { return; }
            }
        }

        private void buttonChangeUserData_Click(object sender, RoutedEventArgs e)
        {
            user = new User();

            YourHeightTextBox.Text = user._Height.ToString();
            YourWeightTextBox.Text = user._Weight.ToString();
            YourAgeTextBox.Text = user._Age.ToString();
            YourCcal.Text = user._Ccal.ToString();

            DetalesOfDailyMenuList.Visibility = Visibility.Collapsed;
        }
    }
}