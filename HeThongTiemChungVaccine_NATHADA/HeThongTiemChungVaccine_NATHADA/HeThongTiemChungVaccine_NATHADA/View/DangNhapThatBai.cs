using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class DangNhapThatBai : Form
    {
        public DangNhapThatBai()
        {
            InitializeComponent();
        }
        int n, i;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label_demlui_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Maximum = n;
            i--;
            this.label_demlui.Text = "Thời gian còn lại là " + i.ToString() + "Giây";

            if (i >= 0)
            {
                progressBar1.Value = i;
            }
            if (i < 0)
            {
                this.timer1.Enabled = false;
                View.DangNhapHeThongTiemChung_NATHADA hehe = new View.DangNhapHeThongTiemChung_NATHADA();
                hehe.Show();
                this.Hide();
            }
        }

        private void DangNhapThatBai_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            i = 100;
            n = i;
        }
    }
}
