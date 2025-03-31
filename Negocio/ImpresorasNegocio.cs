using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    internal class ImpresorasNegocio
    {
        public List<DatosImpresora> ListarImpresoras()
        {
            List<DatosImpresora> listaImpresoras = new List<DatosImpresora>();
            AccesoDatos acceso = new AccesoDatos();

            try
            {
                // Consulta SQL: obtenemos id, modelo (desde la tabla modelos), ip y el texto de la ubicación
                string consulta = @"
                SELECT i.id, m.nombre AS Modelo, i.ip, i.ubicacion_texto AS Ubi 
                FROM impresoras i
                INNER JOIN modelos m ON i.modelo_id = m.id";
                acceso.SetearConsulta(consulta);
                acceso.EjecutarLectura();

                while (acceso.Lector.Read())
                {
                    DatosImpresora impresora = new DatosImpresora
                    {
                        Id = (int)acceso.Lector["id"],
                        Model = acceso.Lector["Modelo"] != DBNull.Value ? (string)acceso.Lector["Modelo"] : string.Empty,
                        ip = acceso.Lector["ip"] != DBNull.Value ? (string)acceso.Lector["ip"] : string.Empty,
                        ubi = acceso.Lector["Ubi"] != DBNull.Value ? (string)acceso.Lector["Ubi"] : string.Empty
                    };

                    listaImpresoras.Add(impresora);
                }

                return listaImpresoras;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar impresoras", ex);
            }
            finally
            {
                acceso.CerrarConexion();
            }

        }

        public List<DatosImpresora> ListarImpresorasPorUbicacion(int ubicacionId)
        {
            List<DatosImpresora> listaImpresoras = new List<DatosImpresora>();
            AccesoDatos acceso = new AccesoDatos();

            try
            {
                string consulta = @"
        SELECT i.id, m.nombre AS Modelo, i.ip, i.ubicacion_texto 
        FROM impresoras i
        INNER JOIN modelos m ON i.modelo_id = m.id
        WHERE i.ubicacion_id = @ubicacionId";

                acceso.SetearConsulta(consulta);
                acceso.SetearParametro("@ubicacionId", ubicacionId);
                acceso.EjecutarLectura();

                while (acceso.Lector.Read())
                {
                    DatosImpresora impresora = new DatosImpresora
                    {
                        Id = (int)acceso.Lector["id"],
                        Model = acceso.Lector["Modelo"]?.ToString(),
                        ip = acceso.Lector["ip"]?.ToString(),
                        ubi = acceso.Lector["ubicacion_texto"]?.ToString()
                    };

                    listaImpresoras.Add(impresora);
                }

                return listaImpresoras;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar impresoras por ubicación", ex);
            }
            finally
            {
                acceso.CerrarConexion();
            }
        }
    }
}