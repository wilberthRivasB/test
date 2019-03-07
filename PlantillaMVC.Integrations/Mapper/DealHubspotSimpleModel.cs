using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Integrations
{
    public class DealListModel {
        public IList<DealModel> Deals { get; set; }
        public bool HasMore { get; set; }
        public long Offset { get; set; }
    }
    public class DealModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
        public string Pipeline { get; set; }
        public long? OwnerId { get; set; }
        public string CloseDate { get; set; }
        public double? Amount { get; set; }
        public string DealType { get; set; }
    }
}
