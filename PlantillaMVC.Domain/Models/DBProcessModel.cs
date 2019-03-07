using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantillaMVC.Domain.Models
{
    public class DBProceso
    {
        public int ProcesoId { set; get; }
        public string Codigo { set; get; }

        public bool EstatusProceso { set; get; }

        public bool EstatusEjecucion { set; get; }

        public DateTime? UltimaEjecucion { set; get; }

        public string Resultado { set; get; }
    }

    public class DBProcesoEjecucion
    {
        public int ProcesoId { set; get; }

        public int ProcesoEjecucionId { set; get; }
        
        public DateTime? FechaInicio { set; get; }

        public DateTime? FechaFin { set; get; }

        public bool Estatus { set; get; }

        public string Resultado { set; get; }
    }
}
