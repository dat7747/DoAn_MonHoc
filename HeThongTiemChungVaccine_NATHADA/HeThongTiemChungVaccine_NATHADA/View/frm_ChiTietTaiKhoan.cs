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
    public partial class frm_ChiTietTaiKhoan : Form
    {
        Control_TaiKhoan control = new Control_TaiKhoan();
        private string tenDangNhap;

        public frm_ChiTietTaiKhoan(string tenDangNhap)
        {
            InitializeComponent();
            this.tenDangNhap = tenDangNhap;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frm_ChiTietTaiKhoan_Load(object sender, EventArgs e)
        {
            DataTable dt = control.LayThongTinNguoiDung(tenDangNhap);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txthoten.Text = row["hoten_nhanvien"].ToString();
                txtdiachi.Text = row["diachi_nhanvien"].ToString();
                txtsodienthoai.Text = row["sdt_nhanvien"].ToString();
                txtemail.Text = row["email_nhanvien"].ToString();
                txtcccd.Text = row["cccd_nhanvien"].ToString();
                txtngaysinh.Text = Convert.ToDateTime(row["ngaysinh_nhanvien"]).ToString("dd/MM/yyyy");
                txtgioitinh.Text = row["gioitinh_nhanvien"].ToString();
                txttendangnhap.Text = row["tendangnhap"].ToString();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btndangxuat_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form chi tiết tài khoản
            Giaodientrangchu mainForm = (Giaodientrangchu)Application.OpenForms["Giaodientrangchu"];
            mainForm.Logout();
        }
    }
}
