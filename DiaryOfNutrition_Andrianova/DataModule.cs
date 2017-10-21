using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DiaryOfSportsNutrition_Andrianova.Abstract;
using DiaryOfSportsNutrition_Andrianova.Entity;
using DiaryOfSportsNutrition_Andrianova.Concrete;

namespace DiaryOfNutrition_Andrianova
{
    public class DataModule : Module
    {
        private string _connStr;
        public DataModule(string connString)
        {
            _connStr = connString;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new EFContext(this._connStr))
                .As<IEFContext>().InstancePerRequest();

            builder.RegisterType<FoodRepository>()
                .As<IFoodRepository>().InstancePerRequest();

            builder.RegisterType<MyPlateRepository>()
                .As<IMyPlateRepository>().InstancePerRequest();

            builder.RegisterType<PlateFoodRecordRepository>()
               .As<IPlateFoodRecordRepository>().InstancePerRequest();

            base.Load(builder);
        }

    }
}
