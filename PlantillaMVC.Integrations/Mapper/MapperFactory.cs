
using PlantillaMVC.Integrations.Hubspot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations.Mapper
{
    public class MapperFactory
    {
        //private IDictionary<Tuple<Type,Type>, IMapper> Mappers = new Dictionary<Tuple<Type, Type>, IMapper>();
        
        public IMapper<TSource, TDestination> CreateMapper<TSource, TDestination>() where TSource : class where TDestination : class
        {
            if(typeof(TSource) == typeof(DealHubSpotResult) && typeof(TDestination) == typeof(DealListModel))
            {
               return (IMapper<TSource, TDestination>) new HubspotModelMapper();
            }
            throw new Exception();
        }
    }
}
