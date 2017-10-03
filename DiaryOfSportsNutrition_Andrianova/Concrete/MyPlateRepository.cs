using DiaryOfSportsNutrition_Andrianova.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Concrete
{
    public class MyPlateRepository : IMyPlateRepository
    {
        private readonly IEFContext _context;
        public MyPlateRepository(IEFContext context)
        {
            _context = context;
        }
        public MyPlate Add(MyPlate myplate)
        {
            _context.Set<MyPlate>().Add(myplate);
            _context.SaveChanges();
            return myplate;
        }

        public MyPlate Remove(MyPlate myplate)
        {
            _context.Set<MyPlate>().Remove(myplate);
            _context.SaveChanges();
            return myplate;
        }

        public IQueryable<MyPlate> MyPlates()
        {
            return _context.Set<MyPlate>().AsQueryable();
        }
    }
}
