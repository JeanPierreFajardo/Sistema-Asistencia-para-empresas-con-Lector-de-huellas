using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;  // Para Enrollment y FeatureExtraction
using UI_Support;  // Si 'AppData' está en este namespace
using DPFP.Verification;

namespace SistemaAsistencia
{
    public class Cls_Huellero : DPFP.Capture.EventHandler
    {
        private Capture Capturer;
        private PictureBox pb_1;
        private const string connectionString = "Data Source=.;Initial Catalog=CA_PERSONAL;Integrated Security=True";
        private byte[] huellaCapturada;
        private DPFP.Template[] Templates = new DPFP.Template[AppData.MaxFingers];
        private AppData Data;
        public Cls_Huellero() 
        {              
            
            Data.OnChange += delegate { ActualizarEstado(); }; // Sincronizar cambios
        }

        // Método para iniciar la captura de huellas
        public void IniciarCaptura()
        {
            try
            {
                if (Capturer != null)
                {
                    DetenerCaptura(); // Detener cualquier captura previa antes de iniciar una nueva
                }

                Capturer = new Capture();

                if (Capturer != null)
                {
                    Capturer.EventHandler = this;
                    Capturer.StartCapture();
                }
                else
                {
                    MessageBox.Show("No se pudo inicializar el lector de huellas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (DPFP.Error.SDKException sdkEx)
            {
                MessageBox.Show("Error en el SDK de huellas: " + sdkEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al iniciar la captura de huellas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Método para detener la captura y liberar recursos
        public void DetenerCaptura()
        {
            try
            {
                if (Capturer != null)
                {
                    Capturer.StopCapture();
                    Capturer.Dispose();  // Liberar recursos del lector
                    Capturer = null;  // Asegurarse de que el objeto sea null para evitar reutilización
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo detener la captura de huellas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Asegurarse de que cualquier recurso gráfico en el PictureBox se libere
                if (pb_1.Image != null)
                {
                    pb_1.Image.Dispose();  // Liberar cualquier imagen previa en el PictureBox
                    pb_1.Image = null;  // Asegurarse de que se reinicie el PictureBox
                }
            }
        }

        // Actualizar el estado en base a los cambios en AppData
        private void ActualizarEstado()
        {
            
        }

        // Cuando se completa la captura
        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            try
            {
                // Convertir la huella a imagen y luego a bytes
                Bitmap huellaImagen = ConvertirSampleAImagen(Sample);
                MostrarHuella(pb_1, huellaImagen);
                huellaCapturada = ConvertirImagenABytes(huellaImagen);

                // Crear el template y almacenarlo en Data
                DPFP.Template template = CrearTemplate(Sample);
                if (template != null)
                {
                    Data.Templates[Data.EnrolledFingersMask] = template;  // Guardar el template en la posición correspondiente
                    Data.EnrolledFingersMask++;  // Actualizar el contador de huellas capturadas
                    Data.Update();  // Notificar el cambio de datos
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error durante la captura de la huella: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DetenerCaptura(); // Detener la captura y liberar recursos al finalizar
            }
        }

        // Crear un template a partir de un sample
        private DPFP.Template CrearTemplate(Sample sample)
        {
            DPFP.Processing.Enrollment enroller = new DPFP.Processing.Enrollment();
            DPFP.FeatureSet features = ExtraerCaracteristicas(sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null)
            {
                enroller.AddFeatures(features);  // Procesar las características extraídas

                if (enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
                {
                    return enroller.Template;
                }
            }

            return null;  // Retorna null si no se pudo crear el template
        }

        // Extraer características de la huella
        private DPFP.FeatureSet ExtraerCaracteristicas(Sample sample, DPFP.Processing.DataPurpose purpose)
        {
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();  // Cambiado de DPFP.FeatureExtraction
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();

            extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);

            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        // Convertir el sample de huella a imagen
        private Bitmap ConvertirSampleAImagen(Sample Sample)
        {
            SampleConversion convertidor = new SampleConversion();
            Bitmap mapaBits = null;
            convertidor.ConvertToPicture(Sample, ref mapaBits); // Convertir la muestra en un mapa de bits
            return mapaBits;
        }

        // Mostrar la huella capturada en el PictureBox
        private void MostrarHuella(PictureBox pictureBox, Bitmap huellaImagen)
        {
            pictureBox.Image = huellaImagen;
        }

        // Convertir la imagen de la huella a un byte array
        private byte[] ConvertirImagenABytes(Bitmap imagen)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imagen.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Guardar la imagen en un stream
                return ms.ToArray(); // Convertir a byte array
            }
        }

        // Método público para obtener la huella capturada
        public byte[] ObtenerHuellaCapturada()
        {
            return huellaCapturada;
        }

        // Manejo de eventos del lector (sin lógica adicional)
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }
    }
}
