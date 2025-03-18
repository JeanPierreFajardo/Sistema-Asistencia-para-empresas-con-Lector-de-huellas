using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using UI_Support;

namespace SistemaAsistencia
{
    public partial class EnrollmentForms : Form
    {
        // Se mantiene la referencia a AppData
        private AppData Data;
        private string connectionString = "Data Source=.;Initial Catalog=CA_PERSONAL;Integrated Security=True";
        private string dniSeleccionado;

        // Constructor que recibe AppData y el DNI
        public EnrollmentForms(AppData data, string dni)
        {
            InitializeComponent();
            Data = data; // Mantener referencia a los datos
            dniSeleccionado = dni;  // Capturar el DNI seleccionado
            ExchangeData(true); // Inicializar los datos con valores predeterminados de los controles
            Data.OnChange += delegate { ExchangeData(false); }; // Seguir los cambios de los datos para mantener el formulario sincronizado

            // Suscripción de eventos
            EnrollmentControl.OnEnroll += EnrollmentControl_OnEnroll;
            EnrollmentControl.OnDelete += EnrollmentControl_OnDelete;
            EnrollmentControl.OnReaderConnect += EnrollmentControl_OnReaderConnect;
            EnrollmentControl.OnReaderDisconnect += EnrollmentControl_OnReaderDisconnect;
            EnrollmentControl.OnFingerRemove += EnrollmentControl_OnFingerRemove;
            EnrollmentControl.OnFingerTouch += EnrollmentControl_OnFingerTouch;
            EnrollmentControl.OnSampleQuality += EnrollmentControl_OnSampleQuality;
            EnrollmentControl.OnComplete += EnrollmentControl_OnComplete;
        }

        // Evento cuando se realiza el enrolamiento
        public void EnrollmentControl_OnEnroll(Object Control, int Finger, DPFP.Template Template, ref DPFP.Gui.EventHandlerStatus Status)
        {
            if (Data.IsEventHandlerSucceeds)
            {
                Data.Templates[Finger - 1] = Template;  // Guardar la plantilla del dedo enrolado
                ExchangeData(true);  // Actualizar otros datos

                // Verificamos si el DNI está asignado correctamente
                if (!string.IsNullOrEmpty(dniSeleccionado))
                {
                    // Guardar la huella directamente en la base de datos
                    GuardarHuellaEnBaseDeDatos(Template);
                    ListEvents.Items.Insert(0, String.Format("OnEnroll: finger {0}", Finger));
                }
                else
                {
                    MessageBox.Show("DNI no válido. No se pudo guardar la huella.");
                }
            }
            else
            {
                Status = DPFP.Gui.EventHandlerStatus.Failure;  // Forzar un estado de error
            }
        }

        // Método para guardar la huella en la base de datos
        private void GuardarHuellaEnBaseDeDatos(DPFP.Template huellaTemplate)
        {
            try
            {
                if (huellaTemplate != null)
                {
                    // Convertir la plantilla a un arreglo de bytes
                    byte[] huellaBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        huellaTemplate.Serialize(ms); // Serializar la huella
                        huellaBytes = ms.ToArray();
                    }

                    // Ahora guardamos o actualizamos los datos en la base de datos
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Personal SET HuellaDactilar = @Huella WHERE DniPer = @Dni", connection))
                        {
                            cmd.Parameters.AddWithValue("@Huella", huellaBytes);  // Asignamos la huella a la columna
                            cmd.Parameters.AddWithValue("@Dni", dniSeleccionado);  // Usamos el DNI seleccionado

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Huella digital guardada o actualizada correctamente.");
                            }
                            else
                            {
                                // Si no se encuentra el registro, mostrar un error
                                MessageBox.Show("No se encontró el registro con ese DNI.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se ha capturado ninguna huella.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar la huella digital: " + ex.Message);
            }
        }

        // Evento para sincronizar el estado visual de los dedos con la máscara actualizada
        public void ExchangeData(bool read)
        {
            if (read)
            {
                Data.EnrolledFingersMask = EnrollmentControl.EnrolledFingerMask;
                Data.MaxEnrollFingerCount = EnrollmentControl.MaxEnrollFingerCount;
                Data.Update();
            }
            else
            {
                EnrollmentControl.EnrolledFingerMask = Data.EnrolledFingersMask; // Aquí se actualiza la interfaz con la máscara correcta
                EnrollmentControl.MaxEnrollFingerCount = Data.MaxEnrollFingerCount;
            }
        }

        // Otros métodos que no requieren cambios
        private void EnrollmentControl_OnDelete(Object Control, int Finger, ref DPFP.Gui.EventHandlerStatus Status) { }
        private void EnrollmentControl_OnReaderConnect(object Control, string ReaderSerialNumber, int Finger) { }
        private void EnrollmentControl_OnReaderDisconnect(object Control, string ReaderSerialNumber, int Finger) { }
        private void EnrollmentControl_OnFingerRemove(object Control, string ReaderSerialNumber, int Finger) { }
        private void EnrollmentControl_OnFingerTouch(object Control, string ReaderSerialNumber, int Finger) { }
        private void EnrollmentControl_OnSampleQuality(object Control, string ReaderSerialNumber, int Finger, DPFP.Capture.CaptureFeedback CaptureFeedback) { }
        private void EnrollmentControl_OnComplete(object Control, string ReaderSerialNumber, int Finger) { }

        // Evento del formulario al cargarse
        private void EnrollmentForms_Load(object sender, EventArgs e)
        {
            this.ListEvents.Items.Clear();
        }

        private void CloseButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
