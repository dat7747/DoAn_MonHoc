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
namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class FrmThongKe : Form
    {
        private ConnSQL connSQL = new ConnSQL();

        public FrmThongKe()
        {
            InitializeComponent();
            InitializeComboBox();
            dataGridView1.ReadOnly = true;

            dateTimePicker1.Visible = false;
            dateTimePicker2.Visible = false;
            button1.Visible = false;
            label3.Visible = false;
            label5.Visible = false;
            label4.Visible = false;
            // Thiết lập sự kiện SelectedIndexChanged cho ComboBox
            comboBoxThongKe.SelectedIndexChanged += comboBoxThongKe_SelectedIndexChanged;
        }
        private void InitializeComboBox()
        {
            comboBoxThongKe.Items.Add("Thống kê số lượng người đăng ký theo ngày");
            comboBoxThongKe.Items.Add("Thống kê theo số lượng Vaccine đã được đăng ký");
            comboBoxThongKe.Items.Add("Thống kê số lượng và doanh thu tiêm chủng");
        }
        private void comboBoxThongKe_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lấy giá trị được chọn từ ComboBox
            string selectedValue = comboBoxThongKe.SelectedItem.ToString();

            // Kiểm tra giá trị được chọn và gọi phương thức thống kê tương ứng
            if (selectedValue == "Thống kê số lượng người đăng ký theo ngày")
            {
                LoadDataThongKeSoluongNguoidangkytheoALLngay();
                dateTimePicker1.Visible = true;
                dateTimePicker1.Enabled = true;
                label3.Visible = true;
                button1.Visible = false;
                label4.Visible = true;
                label4.Text = "Chọn ngày:";
            }
            else if (selectedValue == "Thống kê theo số lượng Vaccine đã được đăng ký")
            {
                LoadDataSoluongVaccineDangky();
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                button1.Visible = false;
                label3.Visible = false;
                label5.Visible = false;
                label4.Visible = false;
            }
            else if (selectedValue == "Thống kê số lượng và doanh thu tiêm chủng")
            {
                ThongKeSoLuongVaDoanhThuTiemChung();
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
                button1.Visible = true;
                button1.Enabled = true;
                label3.Visible = true;
                label5.Visible = true;
                label4.Visible = true;
                label4.Text = "First: ";
            }
        }
        private void FrmThongKe_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ThongKeTheoNgay(dateTimePicker1.Value.Date);
        }
        private void ThongKeTheoNgay(DateTime ngayDangKy)
        {
            using (SqlConnection connection = connSQL.KetNoiCSDL())
            {
                try
                {
                    connection.Open();

                    // Thực hiện truy vấn SQL để thống kê dữ liệu theo ngày đăng ký
                    string query = "SELECT COUNT(*) AS SoLuong FROM NGUOITIEM_DANGKY WHERE ngay_dangky = @NgayDangKy";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NgayDangKy", ngayDangKy);
                    int soLuong = (int)command.ExecuteScalar();

                    MessageBox.Show($"Số lượng đăng ký ngày {ngayDangKy.ToShortDateString()}: {soLuong}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void LoadDataThongKeSoluongNguoidangkytheoALLngay()
        {
            using (SqlConnection connection = connSQL.KetNoiCSDL())
            {
                try
                {
                    connection.Open();

                    // Thực hiện truy vấn SQL để lấy số lượng và loại vaccine đã được đăng ký
                    string query = "SELECT ngay_dangky AS 'Ngày đăng ký', COUNT(*) AS 'Số người người đăng ký' " +
                                    "FROM NGUOITIEM_DANGKY " +
                                    "GROUP BY ngay_dangky";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Đổ dữ liệu vào DataGridView
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Columns[0].Width = 250;
                    dataGridView1.Columns[1].Width = 200;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void ThongKeSoLuongVaDoanhThuTiemChung()
        {
            using (SqlConnection connection = connSQL.KetNoiCSDL())
            {
                try
                {
                    connection.Open();

                    // Thực hiện truy vấn SQL để lấy số lượng và loại vaccine đã được đăng ký
                    string query = "SELECT " +
                                    "ngay_muontiem AS 'Ngày tiêm chủng', " +
                                    "COUNT(dangky.ma_dangky) AS 'Số lượng người đăng ký', " +
                                    "SUM(chitiet.thanhtien) AS 'Doanh thu' " +
                                "FROM " +
                                    "NGUOITIEM_DANGKY dangky " +
                                "INNER JOIN " +
                                    "HOADON hoadon ON dangky.ma_dangky = hoadon.ma_dangky " +
                                "INNER JOIN " +
                                    "CHITIET_HOADON chitiet ON hoadon.ma_hoadon = chitiet.ma_hoadon " +
                                "GROUP BY " +
                                    "ngay_muontiem " +
                                "ORDER BY " +
                                    "ngay_muontiem; ";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Đổ dữ liệu vào DataGridView
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Columns[0].Width = 250;
                    dataGridView1.Columns[1].Width = 100;
                    dataGridView1.Columns[2].Width = 200;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void LoadDataThongKeSoLuongVaDoanhThuTiemChungTheoThoiGian(DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = connSQL.KetNoiCSDL())
            {
                try
                {
                    connection.Open();

                    // Thực hiện truy vấn SQL
                    string query = @"
                SELECT 
                    ngay_muontiem AS 'Ngày tiêm chủng',
                    COUNT(dangky.ma_dangky) AS 'Số lượng người đăng ký',
                    SUM(chitiet.thanhtien) AS 'Doanh thu'
                FROM 
                    NGUOITIEM_DANGKY dangky
                INNER JOIN 
                    HOADON hoadon ON dangky.ma_dangky = hoadon.ma_dangky
                INNER JOIN 
                    CHITIET_HOADON chitiet ON hoadon.ma_hoadon = chitiet.ma_hoadon
                WHERE 
                    ngay_muontiem BETWEEN @StartDate AND @EndDate
                GROUP BY 
                    ngay_muontiem
                ORDER BY 
                    ngay_muontiem";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);


                    // Thực thi truy vấn và đọc kết quả
                    StringBuilder resultBuilder = new StringBuilder();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Đọc dữ liệu từ mỗi dòng và thêm vào chuỗi kết quả
                            string ngayTiemChung = reader["Ngày tiêm chủng"].ToString();
                            int soLuong = Convert.ToInt32(reader["Số lượng người đăng ký"]);
                            decimal doanhThu = Convert.ToDecimal(reader["Doanh thu"]);

                            resultBuilder.AppendLine($"Ngày tiêm chủng: {ngayTiemChung}\nSố lượng: {soLuong}\nDoanh thu: {doanhThu}\n");
                        }
                    }
                    // Hiển thị kết quả trong MessageBox
                    MessageBox.Show(resultBuilder.ToString(), "Kết quả thống kê", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }


        private void LoadDataSoluongVaccineDangky()
        {
            using (SqlConnection connection = connSQL.KetNoiCSDL())
            {
                try
                {
                    connection.Open();

                    // Thực hiện truy vấn SQL để lấy số lượng và loại vaccine đã được đăng ký
                    string query = "SELECT  vc.ten_vaccine as 'Tên Vaccine',  COUNT(*) AS 'Số lượng' " +
                                    " FROM NGUOITIEM_DANGKY dk, VACCINE vc " +
                                    " where dk.ma_vaccine = vc.ma_vaccine " +
                                    " GROUP BY  vc.ten_vaccine";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Đổ dữ liệu vào DataGridView
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Columns[0].Width = 400;
                    dataGridView1.Columns[1].Width = 250;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
