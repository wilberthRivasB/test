using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Domain.Models
{
    public class DBTicketModel
    {
        public string Company { get; set; }
        public DateTime FechaAlta { get; set; }
        public string RFC { get; set; }
        public string TipoActividad { get; set; }
        public decimal? Monto { get; set; }
        public int NumeroOperacion { get; set; }
        public string Descripcion { get; set; }
        public long? TicketId { get; set; }
        public DateTime? SyncDate { get; set; }
    }

   
}
