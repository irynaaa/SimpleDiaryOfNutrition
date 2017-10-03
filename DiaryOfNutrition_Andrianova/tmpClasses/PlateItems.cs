using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DiaryOfNutrition_Andrianova
{
    public class PlateItems
    {
        public DateTime _time { get; set; }
        public string _food { get; set; }
        public float _proteins { get; set; }
        public float _fats { get; set; }
        public float _carbohydrates { get; set; }
        public float _call { get; set; }
        public int _count { get; set; }
    }
}

