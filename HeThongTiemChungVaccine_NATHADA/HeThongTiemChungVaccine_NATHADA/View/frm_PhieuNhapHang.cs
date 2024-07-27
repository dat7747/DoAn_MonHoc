using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
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
    public partial class frm_PhieuNhapHang : Form
    {
        Control_PhieuNhap controlphieunhap = new Control_PhieuNhap();
        private string maNhanVien;
        public frm_PhieuNhapHang(string tenDangNhap)
        {
            Control_NhanVien controlNhanVien = new Control_NhanVien();
            InitializeComponent();
            this.maNhanVien = controlNhanVien.LayMaNhanVienTuTenDangNhap(tenDangNhap);
            cbbcc.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbtenvaccine.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        private void frm_PhieuNhapHang_Load(object sender, EventArgs e)
        {

            LoadNhaCungCap();
            LoadTenVaccine();
        }

        private void LoadNhaCungCap()
        {

            Control_NhaCungCap controlNCC = new Control_NhaCungCap();
            DataTable dtNCC = controlNCC.LayDanhSachNhaCungCap();
            cbbcc.DataSource = dtNCC;
            cbbcc.DisplayMember = "ten_nhacungcap";
            cbbcc.ValueMember = "ma_nhacungcap";
            cbbcc.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadTenVaccine()
        {

            Control_Vaccine controlVaccine = new Control_Vaccine();
            DataTable dtVaccine = controlVaccine.LayDanhSachVaccine();
            cbbtenvaccine.DataSource = dtVaccine;
            cbbtenvaccine.DisplayMember = "ten_vaccine";
            cbbtenvaccine.ValueMember = "ma_vaccine";
            cbbtenvaccine.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void btngui_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra các trường dữ liệu đã được nhập đầy đủ chưa
                if (string.IsNullOrEmpty(cbbtenvaccine.SelectedValue?.ToString()) ||
                    string.IsNullOrEmpty(txtsoluong.Text) ||
                    string.IsNullOrEmpty(cbbcc.SelectedValue?.ToString()))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtsoluong.Text, out int soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là một số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!controlphieunhap.KiemTraTonTaiMaVaccineTrongNhaCungCap(cbbtenvaccine.SelectedValue.ToString(), cbbcc.SelectedValue.ToString()))
                {
                    MessageBox.Show("Nhà cung cấp không có vaccine này.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Xác nhận với người dùng trước khi gửi dữ liệu
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn gửi phiếu nhập và chi tiết phiếu nhập?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Thực hiện gửi dữ liệu
                    Control_PhieuNhap controlPhieuNhap = new Control_PhieuNhap();
                    string maPhieuNhap = controlPhieuNhap.ThemPhieuNhapVaChiTietPhieuNhap(maNhanVien, cbbcc.SelectedValue.ToString(), cbbtenvaccine.SelectedValue.ToString(), soLuong);

                    MessageBox.Show("Thêm phiếu nhập và chi tiết phiếu nhập thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Hiển thị form frm_HienThiPhieuNhapHang với dữ liệu đã yêu cầu
                    frm_HienThiPhieuNhapHang frm = new frm_HienThiPhieuNhapHang(maPhieuNhap);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
