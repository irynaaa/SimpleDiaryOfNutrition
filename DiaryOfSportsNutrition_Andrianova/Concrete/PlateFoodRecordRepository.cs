using DiaryOfSportsNutrition_Andrianova.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Concrete
{
    public class PlateFoodRecordRepository : IPlateFoodRecordRepository
    {
        private readonly IEFContext _context;
        public PlateFoodRecordRepository(IEFContext context)
        {
            _context = context;
        }
        public PlateFoodRecord Add(PlateFoodRecord platefoodrecord)
        {
            _context.Set<PlateFoodRecord>().Add(platefoodrecord);
            _context.SaveChanges();
            return platefoodrecord;
        }

        public IQueryable<PlateFoodRecord> PlateFoodRecords()
        {
            return _context.Set<PlateFoodRecord>().AsQueryable();
        }
    }
}
