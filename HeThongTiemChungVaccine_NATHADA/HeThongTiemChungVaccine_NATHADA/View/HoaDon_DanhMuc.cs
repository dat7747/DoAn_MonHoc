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
    public partial class HoaDon_DanhMuc : Form
    {
        Control_HoaDon controlHoaDon = new Control_HoaDon();
        private string tenDangNhap;
        public HoaDon_DanhMuc(string tenDangNhap)
        {
            InitializeComponent();
            this.tenDangNhap = tenDangNhap;
            AddHeader();
        }

        private void HoaDon_DanhMuc_Load(object sender, EventArgs e)
        {
            DataTable dtHoaDon = controlHoaDon.LayDuLieuHoaDon();

            // Hiển thị dữ liệu lên DataGridView
            dataGridView1.DataSource = dtHoaDon;
            Theater();
        }

        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {    
            DataTable dt = controlHoaDon.TimKiemHoaDonTheoNgay(dateTimePicker1.Value.Date);
            dataGridView1.DataSource = dt;
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

            dataGridView1.Columns.Add("phi_luukho", "Phí Lưu Kho");
            dataGridView1.Columns[9].DataPropertyName = "phi_luukho";

            dataGridView1.Columns.Add("thoigian_thanhtoan", "Thời gian thanh toán");
            dataGridView1.Columns[10].DataPropertyName = "thoigian_thanhtoan";

            dataGridView1.Columns.Add("hinhthuc_thanhtoan", "Hình Thức Thanh Toán");
            dataGridView1.Columns[11].DataPropertyName = "hinhthuc_thanhtoan";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = controlHoaDon.GetHoaDonDataWithStatus1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maHoaDon = dataGridView1.SelectedRows[0].Cells["ma_hoadon"].Value.ToString();
                frm_HienThiHoaDon_XemLai frm = new frm_HienThiHoaDon_XemLai(maHoaDon,tenDangNhap);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
