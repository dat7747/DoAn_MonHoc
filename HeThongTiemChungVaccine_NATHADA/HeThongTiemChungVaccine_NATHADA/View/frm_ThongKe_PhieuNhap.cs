using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_ThongKe_PhieuNhap : Form
    {
        Control_ThongkePhieuNhap control_ThongkePhieuNhap = new Control_ThongkePhieuNhap();
        public frm_ThongKe_PhieuNhap()
        {
            InitializeComponent();
            AddHeader();
            LoadData();
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_phieunhap", "Mã phiếu nhập");
            dataGridView1.Columns[0].DataPropertyName = "ma_phieunhap";

            dataGridView1.Columns.Add("ma_nhanvien", "Mã nhân viên");
            dataGridView1.Columns[1].DataPropertyName = "ma_nhanvien";

            dataGridView1.Columns.Add("hoten_nhanvien", "Họ và tên nhân viên nhập");
            dataGridView1.Columns[2].DataPropertyName = "hoten_nhanvien";

            dataGridView1.Columns.Add("ten_nhacungcap", "Nhà cung cấp");
            dataGridView1.Columns[3].DataPropertyName = "ten_nhacungcap";

            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[4].DataPropertyName = "ten_vaccine";

            dataGridView1.Columns.Add("so_luong", "Số lượng");
            dataGridView1.Columns[5].DataPropertyName = "so_luong";

            dataGridView1.Columns.Add("gia_vaccine", "Giá được nhập của Vaccine");
            dataGridView1.Columns[6].DataPropertyName = "gia_vaccine";

            dataGridView1.Columns.Add("ngay_nhap", "Ngày nhập");
            dataGridView1.Columns[7].DataPropertyName = "ngay_nhap";


            // Thiết lập kích thước cho các cột
            dataGridView1.Columns["ma_phieunhap"].Width = 50;  // Mã phiếu nhập
            dataGridView1.Columns["ma_nhanvien"].Width = 50;   // Mã nhân viên
            dataGridView1.Columns["hoten_nhanvien"].Width = 90; // Họ và tên nhân viên nhập
            dataGridView1.Columns["ten_nhacungcap"].Width = 200; // Nhà cung cấp
            dataGridView1.Columns["ten_vaccine"].Width = 250;    // Tên Vaccine
            dataGridView1.Columns["so_luong"].Width = 40;
            dataGridView1.Columns["gia_vaccine"].Width = 60;     // Giá được nhập của Vaccine
            dataGridView1.Columns["ngay_nhap"].Width = 80;      // Ngày nhập



            // Thiết lập DataGridView chỉ cho phép xem (ReadOnly)
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false; // Không cho phép thêm dòng mới
            dataGridView1.AllowUserToDeleteRows = false; // Không cho phép xóa dòng
            dataGridView1.MultiSelect = false; // Không cho phép chọn nhiều dòng
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically; // Chỉnh sửa bằng code
        }
        void LoadData()
        {
            DataTable dtPhieuNhap = control_ThongkePhieuNhap.LayDuLieuPhieuNhapALL();
            dataGridView1.DataSource = dtPhieuNhap;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        private void frm_ThongKe_PhieuNhap_Load(object sender, EventArgs e)
        {
            Theater();
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
            DataTable dtPhieuNhap = control_ThongkePhieuNhap.LayDuLieuPhieuNhap(ngaybatdau, ngayketthuc);
            dataGridView1.DataSource = dtPhieuNhap;
            // Gọi phương thức để lấy dữ liệu thống kê từ Control_ThongkePhieuNhap
            DataTable dtThongKe = control_ThongkePhieuNhap.LayDuLieuThongKe(ngaybatdau, ngayketthuc);

            // Hiển thị dữ liệu lên các TextBox
            if (dtThongKe.Rows.Count > 0)
            {
                txtsoluongphieunhap.Text = dtThongKe.Rows[0]["SoLuongPhieuNhap"].ToString();
                // Lấy dữ liệu từ DataTable
                decimal tongGiaNhap = Convert.ToDecimal(dtThongKe.Rows[0]["TongGiaNhap"]);

                // Hiển thị số tiền trong TextBox
                txttonggianhap.Text = string.Format(new CultureInfo("vi-VN"), "{0:N0} VNĐ", tongGiaNhap);
            }
            else
            {
                // Nếu không có dữ liệu thống kê thì đặt giá trị rỗng cho các TextBox
                txtsoluongphieunhap.Text = "";
                txttonggianhap.Text = "";
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //// Kiểm tra nếu đây là cột "gia_vaccine" và giá trị hiện tại là số
            //if (dataGridView1.Columns[e.ColumnIndex].Name == "gia_vaccine" && e.Value != null && e.Value != DBNull.Value)
            //{
            //    // Định dạng lại giá trị thành tiền tệ
            //    if (e.Value is int || e.Value is double || e.Value is decimal)
            //    {
            //        e.Value = string.Format("{0:#,##0} VNĐ", Convert.ToDecimal(e.Value));
            //        e.FormattingApplied = true;
            //    }
            //}
        }
    }
}
