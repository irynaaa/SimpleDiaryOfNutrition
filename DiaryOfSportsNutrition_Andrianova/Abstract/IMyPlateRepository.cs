using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Abstract
{
   public interface IMyPlateRepository
    {
        MyPlate Add(MyPlate myplate);
        MyPlate Remove(MyPlate myplate);
        IQueryable<MyPlate> MyPlates();
    }
}
