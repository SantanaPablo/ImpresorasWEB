using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class DatosImpresora
    {
        public int Id { get; set; }
        public string Model { get; set; } //estos tres salen del csv
        public string ip { get; set; } //estos tres salen del csv
        public string ubi { get; set; } //estos tres salen del csv
        public string mac { get; set; }
        public string serial { get; set; }
        public int pageCount { get; set; }
        public int blackToner { get; set; }
        public int cyanToner { get; set; }
        public int magentaToner { get; set; }
        public int yellowToner { get; set; }
        public int wasteContainer { get; set; }
        public int unitImage { get; set; }
        public string status { get; set; }

    }
}
