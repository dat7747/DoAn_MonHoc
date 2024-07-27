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
    public partial class Giaodientrangchu : Form
    {
        public Giaodientrangchu()
        {
            InitializeComponent();
        }


        private void hienthi(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(form);
            this.panel1.Tag = form;

            form.BringToFront();
            form.Show();
        }

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_QL_KhachHang frm = new frm_QL_KhachHang();

            hienthi(frm);
        }

        private void qLLoạiVaccineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoaiVaccine_DanhMuc frmVaccine = new LoaiVaccine_DanhMuc();
            hienthi(frmVaccine);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NhaCungCap_DanhMuc frmNCC = new NhaCungCap_DanhMuc();
            hienthi(frmNCC);
        }

        private void QLNhanvien_Click(object sender, EventArgs e)
        {
            NhanVien_DanhMuc frmNV = new NhanVien_DanhMuc();
            hienthi(frmNV);
        }

        private void QL_Voucher_Click(object sender, EventArgs e)
        {
            Voucher_DanhMuc frmV = new Voucher_DanhMuc();
            hienthi(frmV);
        }

        private void QL_Kho_Click(object sender, EventArgs e)
        {
            Kho_DanhMuc frmK = new Kho_DanhMuc();
            hienthi(frmK);
        }

        private void QL_HoaDon_Click(object sender, EventArgs e)
        {
            HoaDon_DanhMuc frmHD = new HoaDon_DanhMuc();
            hienthi(frmHD);
        }

    }
}
