using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage.Job.Models
{
    public class tb_Referente
    {
        public int ID { get; set; }
        public int IdReferente { get; set; }
        public string Referente { get; set; }
        //public string Login { get; set; }
        public bool Amministratore { get; set; }
        //public bool RefIn { get; set; }
        public bool Smistatore { get; set; }
        public string Email { get; set; }
    }
}