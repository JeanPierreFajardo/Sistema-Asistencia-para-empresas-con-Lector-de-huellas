using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaAsistencia
{
    public partial class Frm_inicial : Form
    {
        public Frm_inicial()
        {
            InitializeComponent();
        }

        private void Btn_ingresar_Click(object sender, EventArgs e)
        {
            Frm_Login gaa = new Frm_Login();
            gaa.Show();
            this.Close();
        }
    }
}
