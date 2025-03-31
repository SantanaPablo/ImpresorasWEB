using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Oids
    {
        public string oidMac { get; set; } //estos oids se cargaran desde un csv depende el modelo
        public string oidModel { get; set; }
        public string oidSerial { get; set; }
        public string oidPageCount { get; set; }
        public string oidBlackTonerFULL { get; set; }
        public string oidCyanTonerFULL { get; set; }
        public string oidMagentaTonerFULL { get; set; }
        public string oidYellowTonerFULL { get; set; }
        public string oidBlackToner { get; set; }
        public string oidCyanToner { get; set; }
        public string oidMagentaToner { get; set; }
        public string oidYellowToner { get; set; }

        public string oidWasteContainer { get; set; }
        public string oidUnitImageFULL { get; set; }
        public string oidUnitImage { get; set; }



    }
}