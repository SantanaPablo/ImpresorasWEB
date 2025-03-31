using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    internal class OidsNegocio
    {

        public Dictionary<string, Oids> ListarOids()
        {
            Dictionary<string, Oids> oidsDictionary = new Dictionary<string, Oids>();
            AccesoDatos acceso = new AccesoDatos();

            try
            {
                // Consulta SQL: obtenemos el nombre del modelo junto con los OIDs
                string consulta = @"
                SELECT m.nombre AS Modelo,
                       o.oid_mac, o.oid_model, o.oid_serial, o.oid_page_count,
                       o.oid_black_toner_full, o.oid_cyan_toner_full, o.oid_magenta_toner_full, o.oid_yellow_toner_full,
                       o.oid_black_toner, o.oid_cyan_toner, o.oid_magenta_toner, o.oid_yellow_toner,
                       o.oid_waste_container, o.oid_unit_image_full, o.oid_unit_image
                FROM oids o
                INNER JOIN modelos m ON o.modelo_id = m.id";
                acceso.SetearConsulta(consulta);
                acceso.EjecutarLectura();

                while (acceso.Lector.Read())
                {
                    string modelo = acceso.Lector["Modelo"] != DBNull.Value ? (string)acceso.Lector["Modelo"] : string.Empty;
                    Oids oid = new Oids
                    {
                        oidMac = acceso.Lector["oid_mac"] != DBNull.Value ? (string)acceso.Lector["oid_mac"] : string.Empty,
                        oidModel = acceso.Lector["oid_model"] != DBNull.Value ? (string)acceso.Lector["oid_model"] : string.Empty,
                        oidSerial = acceso.Lector["oid_serial"] != DBNull.Value ? (string)acceso.Lector["oid_serial"] : string.Empty,
                        oidPageCount = acceso.Lector["oid_page_count"] != DBNull.Value ? (string)acceso.Lector["oid_page_count"] : string.Empty,
                        oidBlackTonerFULL = acceso.Lector["oid_black_toner_full"] != DBNull.Value ? (string)acceso.Lector["oid_black_toner_full"] : string.Empty,
                        oidCyanTonerFULL = acceso.Lector["oid_cyan_toner_full"] != DBNull.Value ? (string)acceso.Lector["oid_cyan_toner_full"] : string.Empty,
                        oidMagentaTonerFULL = acceso.Lector["oid_magenta_toner_full"] != DBNull.Value ? (string)acceso.Lector["oid_magenta_toner_full"] : string.Empty,
                        oidYellowTonerFULL = acceso.Lector["oid_yellow_toner_full"] != DBNull.Value ? (string)acceso.Lector["oid_yellow_toner_full"] : string.Empty,
                        oidBlackToner = acceso.Lector["oid_black_toner"] != DBNull.Value ? (string)acceso.Lector["oid_black_toner"] : string.Empty,
                        oidCyanToner = acceso.Lector["oid_cyan_toner"] != DBNull.Value ? (string)acceso.Lector["oid_cyan_toner"] : string.Empty,
                        oidMagentaToner = acceso.Lector["oid_magenta_toner"] != DBNull.Value ? (string)acceso.Lector["oid_magenta_toner"] : string.Empty,
                        oidYellowToner = acceso.Lector["oid_yellow_toner"] != DBNull.Value ? (string)acceso.Lector["oid_yellow_toner"] : string.Empty,
                        oidWasteContainer = acceso.Lector["oid_waste_container"] != DBNull.Value ? (string)acceso.Lector["oid_waste_container"] : string.Empty,
                        oidUnitImageFULL = acceso.Lector["oid_unit_image_full"] != DBNull.Value ? (string)acceso.Lector["oid_unit_image_full"] : string.Empty,
                        oidUnitImage = acceso.Lector["oid_unit_image"] != DBNull.Value ? (string)acceso.Lector["oid_unit_image"] : string.Empty
                    };

                   
                    oidsDictionary[modelo] = oid;
                }

                return oidsDictionary;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar OIDs", ex);
            }
            finally
            {
                acceso.CerrarConexion();
            }
        }
    }
}
