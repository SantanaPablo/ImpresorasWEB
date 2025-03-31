using System.Collections.Generic;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Negocio;

namespace BuscadorImpresorasWeb.Pages
{
    public class PrintersModel : PageModel
    {
        private readonly SNMPHelper _snmpHelper = new SNMPHelper();
        private readonly UbicacionNegocio _ubicacionNegocio = new UbicacionNegocio();

        public List<DatosImpresora> Printers { get; set; } = new List<DatosImpresora>();
        public List<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();

        [BindProperty(SupportsGet = true)]
        public int UbicacionFilter { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string ModelFilter { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string LocationFilter { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string IpFilter { get; set; } = "";

        public void OnGet()
        {
            //_snmpHelper.LoadPrintersFromCsv("recursos/impresoras.csv");
            //_snmpHelper.LoadOidsFromCsv("recursos/oids.csv");
            //Printers = _snmpHelper.GetPrintersInfo();
            Ubicaciones = _ubicacionNegocio.ListarUbicaciones();

            if (UbicacionFilter == 0)
            {
                Printers = _snmpHelper.GetPrintersInfoFromMySQL();
            }
            else
            {
                Printers = _snmpHelper.GetPrintersInfoFromMySQL(UbicacionFilter);
            }

            if (!string.IsNullOrEmpty(ModelFilter))
                Printers = Printers.FindAll(p => p.Model.Contains(ModelFilter, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(LocationFilter))
                Printers = Printers.FindAll(p => p.ubi.Contains(LocationFilter, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(IpFilter))
                Printers = Printers.FindAll(p => p.ip.Contains(IpFilter, StringComparison.OrdinalIgnoreCase));
        }

        public IActionResult OnPostUpdate()
        {
            //_snmpHelper.LoadPrintersFromCsv("recursos/impresoras.csv");
            //_snmpHelper.LoadOidsFromCsv("recursos/oids.csv");
            //Printers = _snmpHelper.GetPrintersInfo();
            Ubicaciones = _ubicacionNegocio.ListarUbicaciones();
            if (UbicacionFilter == 0)
            {
                Printers = _snmpHelper.GetPrintersInfoFromMySQL();
            }
            else
            {
                Printers = _snmpHelper.GetPrintersInfoFromMySQL(UbicacionFilter);
            }

            if (!string.IsNullOrEmpty(ModelFilter))
                Printers = Printers.FindAll(p => p.Model.Contains(ModelFilter, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(LocationFilter))
                Printers = Printers.FindAll(p => p.ubi.Contains(LocationFilter, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(IpFilter))
                Printers = Printers.FindAll(p => p.ip.Contains(IpFilter, StringComparison.OrdinalIgnoreCase));

            return Page();

        }
    }
}