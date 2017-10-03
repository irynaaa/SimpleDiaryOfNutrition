using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova
{
    public class PlateFoodRecord
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public int PlateId { get; set; }
        public float Weight { get; set; }

        public PlateFoodRecord()
        {
            MyPlate = new List<MyPlate>();
            Food = new List<Food>();
        }

        public virtual ICollection<MyPlate> MyPlate { get; set; }

        public virtual ICollection<Food> Food { get; set; }
    }
}
