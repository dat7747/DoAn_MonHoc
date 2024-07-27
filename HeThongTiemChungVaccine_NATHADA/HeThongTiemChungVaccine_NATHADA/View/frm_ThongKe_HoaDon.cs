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
    public partial class frm_ThongKe_HoaDon : Form
    {
        Control_HoaDon controlHoaDon = new Control_HoaDon();
        public frm_ThongKe_HoaDon()
        {
            InitializeComponent();
            DataTable dtHoaDon = controlHoaDon.LayDuLieuHoaDon();
            dataGridView1.ReadOnly = true;
            // Hiển thị dữ liệu lên DataGridView
            dataGridView1.DataSource = dtHoaDon;
            AddHeader();
        }

        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_hoadon", "Mã Hóa Đơn");
            dataGridView1.Columns[0].DataPropertyName = "ma_hoadon";

            dataGridView1.Columns.Add("hoten_khachhang", "Họ và Tên");
            dataGridView1.Columns[1].DataPropertyName = "hoten_khachhang";

            dataGridView1.Columns.Add("ten_voucher", "Tên Voucher");
            dataGridView1.Columns[2].DataPropertyName = "ten_voucher";

            dataGridView1.Columns.Add("ten_combo", "Tên Combo");
            dataGridView1.Columns[3].DataPropertyName = "ten_combo";

            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[4].DataPropertyName = "ten_vaccine";

            dataGridView1.Columns.Add("soluong_vaccine", "Số lượng Vaccine");
            dataGridView1.Columns[5].DataPropertyName = "soluong_vaccine";

            dataGridView1.Columns.Add("gia_vacine", "Giá Vaccine");
            dataGridView1.Columns[6].DataPropertyName = "gia_vacine";

            dataGridView1.Columns.Add("thanhtien", "Thành Tiền");
            dataGridView1.Columns[7].DataPropertyName = "thanhtien";

            dataGridView1.Columns.Add("chietkhau", "Chiết Khấu");
            dataGridView1.Columns[8].DataPropertyName = "chietkhau";

            dataGridView1.Columns.Add("philuukho", "Phí Lưu Kho");
            dataGridView1.Columns[9].DataPropertyName = "philuukho";

            dataGridView1.Columns.Add("thoigian_thanhtoan", "Thời gian thanh toán");
            dataGridView1.Columns[10].DataPropertyName = "thoigian_thanhtoan";

            dataGridView1.Columns.Add("hinhthuc_thanhtoan", "Hình Thức Thanh Toán");
            dataGridView1.Columns[11].DataPropertyName = "hinhthuc_thanhtoan";
        }
        private void frm_ThongKe_HoaDon_Load(object sender, EventArgs e)
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

            var (dt, tongHoaDon, tongThanhToan) = controlHoaDon.ThongKeHoaDon(ngaybatdau, ngayketthuc);

            dataGridView1.DataSource = dt;
            txtsoluonghoadon.Text = tongHoaDon.ToString();

            // Định dạng tổng thanh toán thành tiền tệ Việt Nam
            CultureInfo viVNCulture = new CultureInfo("vi-VN");
            txttongthanhtoan.Text = tongThanhToan.ToString("C", viVNCulture);
        }
    }
}
