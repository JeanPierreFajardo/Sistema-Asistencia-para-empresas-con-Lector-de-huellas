using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Cls_EditarPersonal
{
    private string connectionString;

    public object DniSeleccionado { get; private set; }

    public Cls_EditarPersonal(string connString)
    {
        connectionString = connString;
    }

    public void CargarDatosEmpleado(string DniSeleccionado, TextBox txt_dni, TextBox txt_id, TextBox txt_nombre, DateTimePicker dt_fecha, ComboBox cb_sexo, TextBox txt_direccion, TextBox txt_correo, TextBox txt_telefono, ComboBox cb_rol, ComboBox cb_distrito, PictureBox pb_foto)
    {
        DataTable datosEmpleado = ObtenerDatosPorDNI(DniSeleccionado);

        if (datosEmpleado.Rows.Count > 0)
        {
            DataRow empleado = datosEmpleado.Rows[0];

            txt_dni.Text = empleado["DniPer"].ToString();
            txt_id.Text = empleado["Id_Pernl"].ToString();

            // Concatenar Nombre, ApellidoPaterno y ApellidoMaterno
            string nombre = empleado["Nombre"].ToString();
            string apellidoPaterno = empleado["ApellidoPaterno"].ToString();
            string apellidoMaterno = empleado["ApellidoMaterno"].ToString();
            txt_nombre.Text = $"{nombre} {apellidoPaterno} {apellidoMaterno}";

            dt_fecha.Value = Convert.ToDateTime(empleado["Fec_Naci"]);
            cb_sexo.Text = empleado["Sexo"].ToString();
            txt_direccion.Text = empleado["Domicilio"].ToString();
            txt_correo.Text = empleado["Correo"].ToString();
            txt_telefono.Text = empleado["Celular"].ToString();

            int idRol = Convert.ToInt32(empleado["Id_Rol"]);
            int idDistrito = Convert.ToInt32(empleado["Id_Distrito"]);

            string nombreRol = ObtenerNombreRolPorId(idRol);
            string nombreDistrito = ObtenerNombreDistritoPorId(idDistrito);

            cb_rol.Text = nombreRol;
            cb_distrito.Text = nombreDistrito;

            byte[] imagen = ObtenerImagenPorDNI(DniSeleccionado);

            if (imagen != null)
            {
                using (MemoryStream ms = new MemoryStream(imagen))
                {
                    pb_foto.Image = Image.FromStream(ms);
                }
            }
        }
    }

    public void CargarDatosCompletos(string dni, Label lbNombre, Label lbDni, Label lbTodo, PictureBox pbImagen)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT Nombre, ApellidoPaterno, ApellidoMaterno, DniPer, Foto FROM Personal WHERE DniPer = @Dni";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Dni", dni);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string nombre = reader["Nombre"].ToString();
                    string apellidoPaterno = reader["ApellidoPaterno"].ToString();
                    string apellidoMaterno = reader["ApellidoMaterno"].ToString();
                    lbNombre.Text = $"{nombre} {apellidoPaterno} {apellidoMaterno}";

                    lbDni.Text = reader["DniPer"].ToString();
                    lbTodo.Text = lbNombre.Text.Substring(0, Math.Min(5, lbNombre.Text.Length)) + lbDni.Text.Substring(0, Math.Min(3, lbDni.Text.Length));

                    // Cargar imagen
                    if (reader["Foto"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])reader["Foto"];
                        using (MemoryStream ms = new MemoryStream(imgData))
                        {
                            pbImagen.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pbImagen.Image = null; // No hay imagen disponible
                    }
                }
                else
                {
                    // Manejo de caso en el que no se encuentra el DNI
                    lbNombre.Text = "No encontrado";
                    lbDni.Text = "";
                    lbTodo.Text = "";
                    pbImagen.Image = null;
                }
            }
        }
    }

    private int ObtenerIdPernlPorDNI(string dni)
    {
        int idPernl = -1;

        string query = "SELECT Id_Pernl FROM Personal WHERE DniPer = @DNI";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DNI", dni);
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    idPernl = Convert.ToInt32(result);
                }
            }
        }

        return idPernl;
    }

    private byte[] ObtenerImagenPorDNI(string dni)
    {
        byte[] imagen = null;

        string query = "SELECT Foto FROM Personal WHERE DniPer = @DNI";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DNI", dni);
                connection.Open();
                var result = command.ExecuteScalar() as byte[];
                if (result != null)
                {
                    imagen = result;
                }
            }
        }

        return imagen;
    }

    private string ObtenerNombreRolPorId(int idRol)
    {
        string nombreRol = string.Empty;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT NomRol FROM Rol WHERE Id_Rol = @IdRol";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdRol", idRol);

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    nombreRol = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        return nombreRol;
    }

    private string ObtenerNombreDistritoPorId(int idDistrito)
    {
        string nombreDistrito = string.Empty;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Distrito FROM Distrito WHERE Id_Distrito = @IdDistrito";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdDistrito", idDistrito);

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    nombreDistrito = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        return nombreDistrito;
    }

    private DataTable ObtenerDatosPorDNI(string dni)
    {
        DataTable datosEmpleado = new DataTable();

        string query = "SELECT * FROM Personal WHERE DniPer = @DNI";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DNI", dni);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(datosEmpleado);
            }
        }

        return datosEmpleado;
    }
}
