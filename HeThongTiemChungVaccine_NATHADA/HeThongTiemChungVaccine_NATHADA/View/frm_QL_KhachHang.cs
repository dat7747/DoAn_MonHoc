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
using HeThongTiemChungVaccine_NATHADA.Model;
using System.Text.RegularExpressions;

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
            btnluu.Enabled = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 100;
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
            {
                dataGridView1.DataSource = null; // Thiết lập DataSource về null trước khi xóa các dòng
                dataGridView1.Rows.Clear(); // Xóa các dòng nếu có
            }
            DataTable dataTable = controlKhachHang.select(table);
            dataGridView1.DataSource = dataTable;
            key[0] = dataTable.Columns[0];
            dataTable.PrimaryKey = key;
            AddHeader();
            Theater();
        }

        void loadAllKhachHang()
        {
            AddHeader();
            LoadKhachHangData();
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }


        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frm_QL_KhachHang_Load(object sender, EventArgs e)
        {
            loadAllKhachHang();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Hiển thị thông tin tương ứng trong các control
                label_tenkh.Text = row.Cells["hoten_khachhang"].Value.ToString();
                label_sdt.Text = row.Cells["sdt_khachhang"].Value.ToString();
                label_gmail.Text = row.Cells["email_khachhang"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["ngaysinh_khachhang"].Value);
                label_gioitinh.Text = row.Cells["gioitinh_khachhang"].Value.ToString();
                label_diem.Text = row.Cells["diemthanthiet"].Value.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchValue = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadKhachHangData(); // Hiển thị tất cả dữ liệu nếu ô tìm kiếm trống
            }
            else
            {
                DataTable dataTable = controlKhachHang.searchByPhoneNumber(table, searchValue);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void btnluu_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    Model_KhachHang khachHang = new Model_KhachHang();
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    khachHang.MaKhachHang = selectedRow.Cells[0].Value.ToString();
                    khachHang.HoTenKhachHang = selectedRow.Cells["hoten_khachhang"].Value.ToString();
                    khachHang.SdtKhachHang = selectedRow.Cells["sdt_khachhang"].Value.ToString();
                    khachHang.EmailKhachHang = selectedRow.Cells["email_khachhang"].Value.ToString();
                    khachHang.NgaySinhKhachHang = Convert.ToDateTime(selectedRow.Cells["ngaysinh_khachhang"].Value);
                    khachHang.GioiTinhKhachHang = selectedRow.Cells["gioitinh_khachhang"].Value.ToString();
                    khachHang.PassKhachHang = selectedRow.Cells["pass_khachhang"].Value.ToString();
                    khachHang.DiemThanThiet = Convert.ToSingle(selectedRow.Cells["diemthanthiet"].Value);
                    khachHang.TrangThai = Convert.ToInt32(selectedRow.Cells["trangthai"].Value);
                    // Kiểm tra các thuộc tính
                    if (!IsPhoneNumberValid(khachHang.SdtKhachHang))
                    {
                        MessageBox.Show("Số điện thoại phải bắt đầu bằng 0 và có đủ 10 số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsEmailValid(khachHang.EmailKhachHang))
                    {
                        MessageBox.Show("Email không đúng định dạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsDateOfBirthValid(khachHang.NgaySinhKhachHang))
                    {
                        MessageBox.Show("Ngày sinh không được lớn hơn ngày hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsPasswordValid(khachHang.PassKhachHang))
                    {
                        MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsDiemThanThietValid(khachHang.DiemThanThiet))
                    {
                        MessageBox.Show("Điểm thân thiết phải lớn hơn hoặc bằng 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsTrangThaiValid(khachHang.TrangThai))
                    {
                        MessageBox.Show("Trạng thái chỉ được là 0 hoặc 1.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    controlKhachHang.Update(khachHang, table);
                    MessageBox.Show("Sửa Thành Công");
                    btnluu.Enabled = false;
                    dataGridView1.ReadOnly = true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            btnluu.Enabled = true;
            dataGridView1.ReadOnly = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                // Chỉ có cột "Mã Khách Hàng" là chỉ đọc
                if (column.Name == "ma_khachhang")
                {
                    column.ReadOnly = true;
                }
                else
                {
                    column.ReadOnly = false;
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            loadAllKhachHang();
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
        }

        private bool IsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsDateOfBirthValid(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Now;
        }

        private bool IsPasswordValid(string password)
        {
            return password.Length > 6;
        }

        private bool IsDiemThanThietValid(float diemThanThiet)
        {
            return diemThanThiet >= 0;
        }

        private bool IsTrangThaiValid(int trangThai)
        {
            return trangThai == 0 || trangThai == 1;
        }
    }
}
