using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryOfSportsNutrition_Andrianova.Abstract
{
   public interface IPlateFoodRecordRepository
    {
        PlateFoodRecord Add(PlateFoodRecord platefoodrecord);
        IQueryable<PlateFoodRecord> PlateFoodRecords();
    }
}
