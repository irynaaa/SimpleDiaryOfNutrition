using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova
{
    public class Food
    {
        public Food()
        { }
        public int Id { get; set; }
        public string FoodName { get; set; }
        public float Proteins { get; set; }
        public float Fat { get; set; }
        public float Carbohydrates { get; set; }
        public float CaloricValue { get; set; }

        public int PlateId { get; set; }

        public override string ToString()
        {
            return FoodName;
        }
    }
}
