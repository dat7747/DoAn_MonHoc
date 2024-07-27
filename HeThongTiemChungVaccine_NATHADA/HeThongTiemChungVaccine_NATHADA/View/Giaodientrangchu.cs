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
        public static string LoggedInUserName = DangNhapHeThongTiemChung_NATHADA.UserName;


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
        }

        private void qLLoạiVaccineToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

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

        private void qLVaccineToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void qUẢNLÝKHÁCHHÀNGToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frm_QL_KhachHang frm = new frm_QL_KhachHang();

            hienthi(frm);
        }

        private void vaccineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vaccine_DanhMuc frmHD = new Vaccine_DanhMuc();
            hienthi(frmHD);
        }

        private void loạiVaccineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoaiVaccine_DanhMuc frmVaccine = new LoaiVaccine_DanhMuc();
            hienthi(frmVaccine);
        }

        private void tÀIKHOẢNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ChiTietTaiKhoan frm = new frm_ChiTietTaiKhoan(LoggedInUserName);
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(frm);
            frm.Show();
        }
        public void Logout()
        {
            this.Close();
            DangNhapHeThongTiemChung_NATHADA loginForm = new DangNhapHeThongTiemChung_NATHADA();
            loginForm.Show();
        }

        private void khoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kho_DanhMuc frm = new Kho_DanhMuc();
            hienthi(frm);
        }
        
        private void tạoPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_PhieuNhapHang frm = new frm_PhieuNhapHang(LoggedInUserName);
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(frm);
            frm.Show();
        }

        private void phiếuNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ThongKe_PhieuNhap frm = new frm_ThongKe_PhieuNhap();
            hienthi(frm);
        }

        private void xemHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HoaDon_DanhMuc frm = new HoaDon_DanhMuc(LoggedInUserName);
            hienthi(frm);
        }

        private void hóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ThongKe_HoaDon frm = new frm_ThongKe_HoaDon();
            hienthi(frm);
        }

        private void comboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboVaccine_DanhMuc frm = new ComboVaccine_DanhMuc();
            hienthi(frm);
        }

        private void đăngKýTiêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NguoDangKyTiem frm = new NguoDangKyTiem(LoggedInUserName);
            hienthi(frm);
        }

        private void QL_NhaCungCap_Click(object sender, EventArgs e)
        {
            NhaCungCap_DanhMuc frmNCC = new NhaCungCap_DanhMuc();
            hienthi(frmNCC);
        }

        private void QL_Voucher_Click_1(object sender, EventArgs e)
        {
            Voucher_DanhMuc frmVoC = new Voucher_DanhMuc();
            hienthi(frmVoC);
        }

        private void QL_Nhanvien_Click(object sender, EventArgs e)
        {
            // Sử dụng biến tĩnh UserRole từ form hiện tại
            if (DangNhapHeThongTiemChung_NATHADA.UserRole == "Admin")
            {
                // Chỉ quản trị viên mới có quyền truy cập vào quản lý nhân viên
                NhanVien_DanhMuc frmNV = new NhanVien_DanhMuc();
                hienthi(frmNV);
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập vào chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Giaodientrangchu_Load(object sender, EventArgs e)
        {

        }

        private void hoaDonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_TaoHoaDon frm = new frm_TaoHoaDon(LoggedInUserName);
            hienthi(frm);
        }

        private void trạngTháiTiêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_SetTrangThaiTiem frm = new frm_SetTrangThaiTiem();
            hienthi(frm);
        }

        private void QL_TaiKhoanNhanVien_Click(object sender, EventArgs e)
        {
            // Sử dụng biến tĩnh UserRole từ form hiện tại
            if (DangNhapHeThongTiemChung_NATHADA.UserRole == "Admin")
            {
                // Chỉ quản trị viên mới có quyền truy cập vào quản lý nhân viên
                TaiKhoanNhanVien_DanhMuc frmTKNV = new TaiKhoanNhanVien_DanhMuc();
                hienthi(frmTKNV);
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập vào chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ngườiĐăngKýTiêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ThongKe_NguoiDangKy frm = new frm_ThongKe_NguoiDangKy();
            hienthi(frm);
        }

        private void lịchSửTiêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Thongke_LichSuTiem frm = new frm_Thongke_LichSuTiem();
            hienthi(frm);
        }

        private void lịchSửMuaHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ThongKe_lichsumuahang frm = new frm_ThongKe_lichsumuahang();
            hienthi(frm);
        }
    }
}
