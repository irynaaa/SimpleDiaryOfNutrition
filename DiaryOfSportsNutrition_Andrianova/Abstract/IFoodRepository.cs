using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Abstract
{
    public interface IFoodRepository
    {
        Food Add(Food food);
        IQueryable<Food> Products();
    }
}
