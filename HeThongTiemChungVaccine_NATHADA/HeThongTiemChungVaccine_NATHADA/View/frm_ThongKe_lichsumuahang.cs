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
    public partial class frm_ThongKe_lichsumuahang : Form
    {
        Control_ThongkeLichSuMuaHang control_Thongke = new Control_ThongkeLichSuMuaHang();
        public frm_ThongKe_lichsumuahang()
        {
            InitializeComponent();
            AddHeader();
            LoadData();
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_hoadon", "Mã hóa đơn");
            dataGridView1.Columns[0].DataPropertyName = "ma_hoadon";

            dataGridView1.Columns.Add("ma_khachhang", "Mã khách hàng");
            dataGridView1.Columns[1].DataPropertyName = "ma_khachhang";

            dataGridView1.Columns.Add("hoten_khachhang", "Họ và tên khách hàng");
            dataGridView1.Columns[2].DataPropertyName = "hoten_khachhang";

            dataGridView1.Columns.Add("thoigian_thanhtoan", "thời gian thanh toán");
            dataGridView1.Columns[3].DataPropertyName = "thoigian_thanhtoan";


            // Thiết lập DataGridView chỉ cho phép xem (ReadOnly)
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false; // Không cho phép thêm dòng mới
            dataGridView1.AllowUserToDeleteRows = false; // Không cho phép xóa dòng
            dataGridView1.MultiSelect = false; // Không cho phép chọn nhiều dòng
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically; // Chỉnh sửa bằng code
        }
        void LoadData()
        {
            DataTable dtPhieuNhap = control_Thongke.LayDuLieuALL();
            dataGridView1.DataSource = dtPhieuNhap;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime ngaybatdau = dtpkngaybatdau.Value.Date;
            DateTime ngayketthuc = dtpkngayketthuc.Value.Date.AddDays(1).AddTicks(-1);

            // Kiểm tra ngày bắt đầu không lớn hơn ngày kết thúc
            if (ngaybatdau > ngayketthuc)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataTable dtPhieuNhap = control_Thongke.LayDuLieu(ngaybatdau, ngayketthuc);
            dataGridView1.DataSource = dtPhieuNhap;

            DataRow khachHangMax = control_Thongke.LayKhachHangCoTongThanhTienCaoNhat(ngaybatdau, ngayketthuc);
            if (khachHangMax != null)
            {
                string hoTen = khachHangMax["hoten_khachhang"].ToString();
                string maKhachHang = khachHangMax["ma_khachhang"].ToString();
                decimal tongThanhTien = Convert.ToDecimal(khachHangMax["Tổng thành tiền"]);

                lblKhachHangMax.Text = $"Khách hàng: {hoTen} (Mã KH: {maKhachHang}) Tổng tiền thanh toán: {tongThanhTien:#,##0} VNĐ";
            }
            else
            {
                lblKhachHangMax.Text = "Không có khách hàng nào trong khoảng thời gian được chọn.";
            }

        }

        private void frm_ThongKe_lichsumuahang_Load(object sender, EventArgs e)
        {
            Theater();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra nếu đây là cột "gia_vaccine" và giá trị hiện tại là số
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Tổng thành tiền" && e.Value != null && e.Value != DBNull.Value)
            {
                // Định dạng lại giá trị thành tiền tệ
                if (e.Value is int || e.Value is double || e.Value is decimal)
                {
                    e.Value = string.Format("{0:#,##0} VNĐ", Convert.ToDecimal(e.Value));
                    e.FormattingApplied = true;
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
