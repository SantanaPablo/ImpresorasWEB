using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class UbicacionNegocio
    {
        public List<Ubicacion> ListarUbicaciones()
        {
            List<Ubicacion> listaUbicaciones = new List<Ubicacion>();
            AccesoDatos acceso = new AccesoDatos();

            try
            {
                string consulta = "SELECT id, nombre FROM ubicaciones ORDER BY nombre";
                acceso.SetearConsulta(consulta);
                acceso.EjecutarLectura();


                while (acceso.Lector.Read())
                {
                    Ubicacion ubicacion = new Ubicacion
                    {
                        Id = (int)acceso.Lector["id"],
                        Nombre = acceso.Lector["nombre"].ToString()
                    };

                    listaUbicaciones.Add(ubicacion);
                }

                return listaUbicaciones;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar ubicaciones", ex);
            }
            finally
            {
                acceso.CerrarConexion();
            }
        }
    }
}
