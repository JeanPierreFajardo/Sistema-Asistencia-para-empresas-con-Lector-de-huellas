using System;
using System.Data.SqlClient;
using SistemaAsistencia;

public class Cls_configuracion
{
    private string connectionString = "Data Source=.;Initial Catalog=CA_PERSONAL;Integrated Security=True";

    public Horario ObtenerConfiguracion()
    {
        Horario horario = new Horario();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Horario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        horario.HorEntrada = reader.GetTimeSpan(reader.GetOrdinal("HorEntrada"));
                        horario.MinTolerancia = reader.GetTimeSpan(reader.GetOrdinal("MinTolerancia"));
                        horario.HorLimite = reader.GetTimeSpan(reader.GetOrdinal("HorLimite"));
                        horario.HorSalida = reader.GetTimeSpan(reader.GetOrdinal("HorSalida"));
                    }

                    reader.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener la configuración: " + ex.Message);
        }

        return horario;
    }

    public bool GuardarConfiguracion(TimeSpan horEntrada, TimeSpan minTolerancia, TimeSpan horLimite, TimeSpan horSalida)
    {
        bool guardado = false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Aquí debes agregar una cláusula WHERE en tu consulta UPDATE
                string query = "UPDATE Horario SET HorEntrada = @HorEntrada, MinTolerancia = @MinTolerancia, HorLimite = @HorLimite, HorSalida = @HorSalida WHERE Id_Hor = 1"; // Cambia "1" por el ID del registro que deseas actualizar

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HorEntrada", horEntrada);
                    command.Parameters.AddWithValue("@MinTolerancia", minTolerancia);
                    command.Parameters.AddWithValue("@HorLimite", horLimite);
                    command.Parameters.AddWithValue("@HorSalida", horSalida);

                    int result = command.ExecuteNonQuery();
                    guardado = (result > 0);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar la configuración: " + ex.ToString());
        }

        return guardado;
    }
}
