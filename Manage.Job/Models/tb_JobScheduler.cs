using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage.Job.Models
{
    public class tb_JobScheduler
    {
        // public string Title { get; set; }
        public int ID { get; set; }
        public string GUID { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizioJob { get; set; }
        public DateTime DataFineJob { get; set; }
        public string TipoJob{ get; set; }
        public string Ripetizione { get; set; }
        public string StatoJob { get; set; }
        public int Referente1 { get; set; }
        public string Referente1Valore { get; set; }
        public int Referente2 { get; set; }
        public string Referente2Valore { get; set; }
        public int Referente3 { get; set; }
        public string Referente3Valore { get; set; }
        public int Referente4 { get; set; }
        public string Referente4Valore { get; set; }
        public int Referente5 { get; set; }
        public string Referente5Valore { get; set; }

        public string Autore { get; set; }
        public DateTime DataCreazione { get; set; }

        public string AutoreUltimaModifica { get; set; }
        public DateTime DataUltimaModifica { get; set; }

    }
}
