using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SistemaAsistencia
{
    class Cls_SolicJustificaciones
    {
        private string connectionString;

        public Cls_SolicJustificaciones(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ActualizarJustificacion(int idJustificacion, string motivo, string detalle, DateTime fechaJustificacion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("UPDATE Justificacion SET PrincipalMotivo = @PrincipalMotivo, Detalle_Justi = @Detalle_Justi, FechaJusti = @FechaJusti WHERE Id_Justi = @Id_Justi", connection))
                    {
                        command.Parameters.AddWithValue("@Id_Justi", idJustificacion);
                        command.Parameters.AddWithValue("@PrincipalMotivo", motivo);
                        command.Parameters.AddWithValue("@Detalle_Justi", detalle);
                        command.Parameters.AddWithValue("@FechaJusti", fechaJustificacion);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la justificación: " + ex.ToString());
                return false;
            }
        }

        public DataTable ObtenerDatosJustificacionPorNombre(string nombreCompleto)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ObtenerDatosJustificacionPorNombre", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Asumiendo que el nombre completo contiene Nombre, ApellidoPaterno y ApellidoMaterno
                        string[] nombres = nombreCompleto.Split(' ');
                        if (nombres.Length >= 3)
                        {
                            command.Parameters.AddWithValue("@Nombre", nombres[0]);
                            command.Parameters.AddWithValue("@ApellidoPaterno", nombres[1]);
                            command.Parameters.AddWithValue("@ApellidoMaterno", nombres[2]);
                        }
                        else if (nombres.Length == 2)
                        {
                            command.Parameters.AddWithValue("@Nombre", nombres[0]);
                            command.Parameters.AddWithValue("@ApellidoPaterno", nombres[1]);
                            command.Parameters.AddWithValue("@ApellidoMaterno", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Nombre", nombreCompleto);
                            command.Parameters.AddWithValue("@ApellidoPaterno", DBNull.Value);
                            command.Parameters.AddWithValue("@ApellidoMaterno", DBNull.Value);
                        }

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener datos de justificación: " + ex.Message);
            }

            return dataTable;
        }

        public DataTable ObtenerJustificaciones()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ObtenerJustificaciones", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter adapter = new SqlDataAdapter(command);

                        connection.Open();
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show("Error al obtener justificaciones: " + ex.Message);
                return null;
            }

            return dataTable;
        }

        public int ObtenerIdPernlFromDni(string dni)
        {
            int idPernl = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id_Pernl FROM Personal WHERE DniPer = @dni";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        idPernl = Convert.ToInt32(result);
                    }
                }
            }
            return idPernl;
        }

        public string ObtenerNombreFromDni(string dni)
        {
            string nombre = "Nombre por defecto";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Nombre + ' ' + ApellidoPaterno + ' ' + ApellidoMaterno FROM Personal WHERE DniPer = @dni";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        nombre = result.ToString();
                    }
                }
            }

            return nombre;
        }
    }
}
