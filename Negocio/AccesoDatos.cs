using System;
using MySql.Data.MySqlClient;

namespace Negocio
{
    public class AccesoDatos
    {
        private MySqlConnection conexion;
        private MySqlCommand comando;
        private MySqlDataReader lector;

        public MySqlDataReader Lector => lector;

        
        public AccesoDatos()
        {
         
            string cadenaConexion = "server=localhost; database=impresoras; user id=root; password=amarazul77";
            conexion = new MySqlConnection(cadenaConexion);
            comando = new MySqlCommand();
        }

       
        public void SetearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

       
        public void EjecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la lectura de datos", ex);
            }
        }

       
        public void EjecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la acción en la base de datos", ex);
            }
        }

      
        public void SetearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }

      
        public void CerrarConexion()
        {
            if (lector != null && !lector.IsClosed)
                lector.Close();
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
                conexion.Close();
        }
    }
}
