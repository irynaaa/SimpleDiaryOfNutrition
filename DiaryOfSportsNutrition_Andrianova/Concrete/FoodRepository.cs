using DiaryOfSportsNutrition_Andrianova.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Concrete
{
    public class FoodRepository : IFoodRepository
    {
        private readonly IEFContext _context;
        public FoodRepository(IEFContext context)
        {
            _context = context;
        }
        public Food Add(Food food)
        {
            _context.Set<Food>().Add(food);
            _context.SaveChanges();
            return food;
        }

        public IQueryable<Food> Products()
        {
            return _context.Set<Food>().AsQueryable();
        }
    }
}
