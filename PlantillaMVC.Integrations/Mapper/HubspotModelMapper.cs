using PlantillaMVC.Integrations.Hubspot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations.Mapper
{
    public class HubspotModelMapper : IMapper<DealHubSpotResult, DealListModel>
    {
        public DealListModel Map(DealHubSpotResult objToMap)
        {
            DealListModel listModel = new DealListModel {
                HasMore = objToMap.HasMore,
                Offset = objToMap.Offset
            };
            listModel.Deals = objToMap.Deals.Select(x => new DealModel {
                Amount = (x.Properties.Amount!=null && !string.IsNullOrEmpty(x.Properties.Amount.Value)) ? (double?)Double.Parse(x.Properties.Amount.Value) : null ,
                //CloseDate = x.Properties.CloseDate.Value,
                Name = x.Properties.Dealname.Value,
                Stage = x.Properties.DealStage.Value,
                Id = x.DealId,
                OwnerId = (x.Properties.HubspotOwnerId == null && !string.IsNullOrEmpty(x.Properties.HubspotOwnerId.Value)) ? null : (long?)Int64.Parse(x.Properties.HubspotOwnerId.Value)
            }).ToList();
            return listModel;
        }
    }
}
