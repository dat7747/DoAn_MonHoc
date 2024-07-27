using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using HeThongTiemChungVaccine_NATHADA.Control;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_QL_KhachHang : Form
    {
        Control_Khachhang controlKhachHang = new Control_Khachhang();
        DataColumn[] key = new DataColumn[1];
        string table = "KHACHHANG";

        public frm_QL_KhachHang()
        {
            InitializeComponent();
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_khachhang", "Mã Khách Hàng");
            dataGridView1.Columns[0].DataPropertyName = "ma_khachhang";

            dataGridView1.Columns.Add("hoten_khachhang", "Họ và Tên");
            dataGridView1.Columns[1].DataPropertyName = "hoten_khachhang";

            dataGridView1.Columns.Add("sdt_khachhang", "Số Điện Thoại");
            dataGridView1.Columns[2].DataPropertyName = "sdt_khachhang";

            dataGridView1.Columns.Add("email_khachhang", "Email");
            dataGridView1.Columns[3].DataPropertyName = "email_khachhang";

            dataGridView1.Columns.Add("ngaysinh_khachhang", "Ngày Sinh");
            dataGridView1.Columns[4].DataPropertyName = "ngaysinh_khachhang";

            dataGridView1.Columns.Add("gioitinh_khachhang", "Giới Tính");
            dataGridView1.Columns[5].DataPropertyName = "gioitinh_khachhang";

            dataGridView1.Columns.Add("pass_khachhang", "Mật Khẩu");
            dataGridView1.Columns[6].DataPropertyName = "pass_khachhang";

            dataGridView1.Columns.Add("diemthanthiet", "Điểm Thân Thiết");
            dataGridView1.Columns[7].DataPropertyName = "diemthanthiet";

            dataGridView1.Columns.Add("trangthai", "Trạng Thái");
            dataGridView1.Columns[8].DataPropertyName = "trangthai";
        }


        void LoadKhachHangData()
        {
            if (dataGridView1.DataSource != null)
                dataGridView1.Rows.Clear();
            DataTable dataTable = controlKhachHang.select(table);
            dataGridView1.DataSource = dataTable;
            key[0] = dataTable.Columns[0];
            dataTable.PrimaryKey = key;
        }

        void loadAllKhachHang()
        {
            AddHeader();
            LoadKhachHangData();
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void frm_QL_KhachHang_Load(object sender, EventArgs e)
        {
            loadAllKhachHang();
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
