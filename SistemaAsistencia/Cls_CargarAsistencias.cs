using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaAsistencia
{
    public class Cls_CargarAsistencias
    {
        private string connectionString = "Data Source=.;Initial Catalog=CA_PERSONAL;Integrated Security=True";

        public DataTable CargarAsistencia()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("sp_CargarAsistencia", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public DataTable CargarAsistenciaActual()
        {
            DataTable dtAsistencias = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("sp_CargarAsistenciaActual", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtAsistencias);
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según tus necesidades
                MessageBox.Show($"Error al cargar las asistencias: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dtAsistencias;
        }
                
    }
}