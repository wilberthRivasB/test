using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations.Mapper
{
    public interface IMapper<TSource, TDestination> where TSource : class where TDestination : class
    {
        TDestination Map(TSource objToMap);
    }
}
