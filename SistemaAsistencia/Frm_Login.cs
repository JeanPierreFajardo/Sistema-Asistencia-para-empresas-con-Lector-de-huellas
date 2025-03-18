using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemaAsistencia
{
    public partial class Frm_Login : Form
    {
        //METODO PARA HACER MOVIBLE EL PANEL 
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=CA_PERSONAL;Integrated Security=True");

        public Frm_Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aquí podrías agregar cualquier lógica de carga inicial
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Método vacío por ahora, puedes eliminarlo si no lo necesitas
        }

        // Método para arrastrar el panel1 cuando el botón izquierdo del mouse está presionado
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, (IntPtr)0xf012, IntPtr.Zero);
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            // Método vacío por ahora, puedes eliminarlo si no lo necesitas
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CONEXIÓN A MI TABLA PUsuario PARA INICIAR SESIÓN
            string NomUsuario = txt_usuario.Text;
            string Password = txt_contraseña.Text;

            try
            {
                connection.Open();

                // Modificamos la consulta para obtener también el tipo de rol
                string selectQuery = "SELECT PUsuario.Nombre, Rol.NomRol, PUsuario.Avatar, PUsuario.Id_Rol " +
                                     "FROM PUsuario " +
                                     "INNER JOIN Rol ON PUsuario.Id_Rol = Rol.Id_Rol " +
                                     "WHERE NomUsuario = @NomUsuario AND Password = @Password";

                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@NomUsuario", NomUsuario);
                command.Parameters.AddWithValue("@Password", Password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string nombreCompleto = reader["Nombre"].ToString();  // Utiliza solo el campo Nombre
                    string nomRol = reader["NomRol"].ToString();
                    byte[] avatarBytes = (byte[])reader["Avatar"];
                    int tipoRol = Convert.ToInt32(reader["Id_Rol"]); // Obtenemos el tipo de rol como entero

                    MessageBox.Show("Bienvenido " + nombreCompleto);

                    // Pasamos el valor de tipoRol al crear Frm_principal
                    Frm_principal frmPrincipal = new Frm_principal(nombreCompleto, nomRol, avatarBytes, tipoRol);
                    frmPrincipal.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Credenciales inválidas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        private void btn_minimizar_Click(object sender, EventArgs e)
        {
            Msj_Form.Frm_Advertencia advertencia = new Msj_Form.Frm_Advertencia();
            advertencia.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Registrarse registrarseForm = new Registrarse();
            registrarseForm.Show();
        }

        private void btn_registroPorHuella_MouseClick(object sender, MouseEventArgs e)
        {
            Frm_Marcar_Asistencia ga = new Frm_Marcar_Asistencia();
            ga.Show();
        }

        private void btn_registroManual_MouseClick(object sender, MouseEventArgs e)
        {
            Frm_Marcar_Asis_Manual ga = new Frm_Marcar_Asis_Manual();
            ga.Show();
        }
    }
}
