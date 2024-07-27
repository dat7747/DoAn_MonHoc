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
    public partial class frm_ThongKe_NguoiDangKy : Form
    {
        Control_thongke_nguoidangky control_Thongke_Nguoidangky = new Control_thongke_nguoidangky();
        public frm_ThongKe_NguoiDangKy()
        {
            InitializeComponent();
            AddHeader();
            LoadData();

        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_dangky", "Mã đăng ký");
            dataGridView1.Columns[0].DataPropertyName = "ma_dangky";

            dataGridView1.Columns.Add("hoten_nguoilienhe", "Họ tên người đăng ký ");
            dataGridView1.Columns[1].DataPropertyName = "hoten_nguoilienhe";

            dataGridView1.Columns.Add("hoten_nguoitiem", "Họ tên người tiêm ");
            dataGridView1.Columns[2].DataPropertyName = "hoten_nguoitiem";

            dataGridView1.Columns.Add("ngay_dangky", "Ngày đăng ký");
            dataGridView1.Columns[3].DataPropertyName = "ngay_dangky";

            dataGridView1.Columns.Add("ngay_muontiem", "Ngày muốn tiêm");
            dataGridView1.Columns[4].DataPropertyName = "ngay_muontiem";



            // Thiết lập kích thước cho các cột
            dataGridView1.Columns["ma_dangky"].Width = 80;
            //dataGridView1.Columns["hoten_nguoilienhe"].Width = 200;   
            //dataGridView1.Columns["hoten_nguoitiem"].Width = 200; 
            //dataGridView1.Columns["ngay_dangky"].Width = 100; 
            //dataGridView1.Columns["ngay_muontiem"].Width = 100;    




            // Thiết lập DataGridView chỉ cho phép xem (ReadOnly)
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false; // Không cho phép thêm dòng mới
            dataGridView1.AllowUserToDeleteRows = false; // Không cho phép xóa dòng
            dataGridView1.MultiSelect = false; // Không cho phép chọn nhiều dòng
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically; // Chỉnh sửa bằng code


        }
        void LoadData()
        {
            DataTable dtPhieuNhap = control_Thongke_Nguoidangky.LayDuLieuDangKyALL();
            dataGridView1.DataSource = dtPhieuNhap;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        private void frm_ThongKe_NguoiDangKy_Load(object sender, EventArgs e)
        {
            Theater();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
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
            DataTable dt = control_Thongke_Nguoidangky.LayDuLieuDangKy(ngaybatdau, ngayketthuc);
            dataGridView1.DataSource = dt;
            setSoluongLable();
        }
        private void setSoluongLable()
        {
            DateTime ngaybatdau = dtpkngaybatdau.Value.Date;
            DateTime ngayketthuc = dtpkngayketthuc.Value.Date.AddDays(1).AddTicks(-1);
            // Đếm số lượng phiếu đăng ký và hiển thị trên Label khi Form được tải
            int soluong = control_Thongke_Nguoidangky.DemSoLuongDangKy(ngaybatdau, ngayketthuc);
            lblSoLuongDangKy.Text = soluong.ToString();
        }
    }
}
