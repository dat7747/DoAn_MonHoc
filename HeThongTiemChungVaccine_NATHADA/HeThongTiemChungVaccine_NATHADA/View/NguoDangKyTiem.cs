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
using HeThongTiemChungVaccine_NATHADA.Model;
using System.Text.RegularExpressions;
namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class NguoDangKyTiem : Form
    {
        private string tenDangNhap;
        private string maNhanVien;
        ConnSQL connect = new ConnSQL();
        Control_Dangkytiem controlDangkytiem = new Control_Dangkytiem();
        string table = "NGUOITIEM_DANGKY";
        public NguoDangKyTiem(string tenDangNhap)
        {
            Control_NhanVien controlNhanVien = new Control_NhanVien();
            InitializeComponent();
            // Đặt FormBorderStyle thành FixedDialog để ngăn người dùng chỉnh sửa kích thước form
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            this.maNhanVien = controlNhanVien.LayMaNhanVienTuTenDangNhap(tenDangNhap);
            this.tenDangNhap = tenDangNhap;
        }  
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ma_dangky", "Mã ĐK");
            dataGridView1.Columns[0].DataPropertyName = "ma_dangky";
            dataGridView1.Columns.Add("hoten_nguoitiem", "Tên người tiêm");
            dataGridView1.Columns[1].DataPropertyName = "hoten_nguoitiem";
            dataGridView1.Columns.Add("ngaysinh_nguoitiem", "Ngày sinh");
            dataGridView1.Columns[2].DataPropertyName = "ngaysinh_nguoitiem";
            dataGridView1.Columns.Add("gioitinh_nguoitiem", "Giới tính");
            dataGridView1.Columns[3].DataPropertyName = "gioitinh_nguoitiem";
            dataGridView1.Columns.Add("diachi_nguoitiem", "Địa chỉ");
            dataGridView1.Columns[4].DataPropertyName = "diachi_nguoitiem";
            dataGridView1.Columns.Add("hoten_nguoilienhe", "Người liên hệ");
            dataGridView1.Columns[5].DataPropertyName = "hoten_nguoilienhe";
            dataGridView1.Columns.Add("sdt_nguoilienhe", "SĐT");
            dataGridView1.Columns[6].DataPropertyName = "sdt_nguoilienhe";
            dataGridView1.Columns.Add("hoten_khachhang", "Tên khách hàng");
            dataGridView1.Columns[7].DataPropertyName = "hoten_khachhang";
            dataGridView1.Columns.Add("phi_luukho", "Phí lưu kho");
            dataGridView1.Columns[8].DataPropertyName = "phi_luukho";
            dataGridView1.Columns.Add("tongthanhtoan", "Tổng thanh toán");
            dataGridView1.Columns[9].DataPropertyName = "tongthanhtoan";
            dataGridView1.Columns.Add("ngay_dangky", "Ngày đăng ký");
            dataGridView1.Columns[10].DataPropertyName = "ngay_dangky";
            dataGridView1.Columns.Add("ngay_muontiem", "Ngày tiêm");
            dataGridView1.Columns[11].DataPropertyName = "ngay_muontiem";
            dataGridView1.Columns.Add("ma_voucher", "Voucher");
            dataGridView1.Columns[12].DataPropertyName = "ma_voucher";
            dataGridView1.Columns.Add("ma_khachhang", "Mã KH");
            dataGridView1.Columns[13].DataPropertyName = "ma_khachhang";

        }

        private void LoadData()
        {
            try
            {
                DataTable dt = controlDangkytiem.SelectAll();
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void NguoDangKyTiem_Load(object sender, EventArgs e)
        {
            AddHeader();
            LoadData();
            Theater();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string ma_dangky = row.Cells["ma_dangky"].Value.ToString();

                // Tạo thể hiện mới của form chi tiết và truyền mã đăng ký vào constructor
                frm_ChiTietDangKy chiTietForm = new frm_ChiTietDangKy(ma_dangky);
                chiTietForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xem chi tiết", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnlaphoadon_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string ma_dangky = row.Cells["ma_dangky"].Value.ToString();
                string ma_khachhang = row.Cells["ma_khachhang"].Value.ToString();
                string ma_voucher = row.Cells["ma_voucher"].Value?.ToString() ?? string.Empty; // Check for null
                string ma_hoadon = GenerateMaHoaDon();
                float thanhtien = float.Parse(row.Cells["tongthanhtoan"].Value.ToString());

                List<string> maVaccineList = new List<string>();
                List<string> maComboList = new List<string>();

                try
                {
                    Control_Dangkytiem controlDangkytiem = new Control_Dangkytiem();

                    if (controlDangkytiem.KiemTraTonTaiHoaDon(ma_dangky))
                    {
                        MessageBox.Show("Mã đăng ký này đã tồn tại trong hóa đơn. Không thể tạo hóa đơn mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    maVaccineList = controlDangkytiem.GetMaVaccine(ma_dangky);
                    maComboList = controlDangkytiem.GetMaCombo(ma_dangky);

                    string ma_combo = string.Join(",", maComboList);
                    string ma_vaccine = string.Join(",", maVaccineList);

                    DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn lập hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        controlDangkytiem.InsertHoaDon(ma_hoadon, ma_dangky, ma_khachhang, ma_voucher, ma_combo, 1, maNhanVien);

                        controlDangkytiem.InsertChiTietHoaDon(ma_hoadon, ma_vaccine, thanhtien);

                        InsertTiemVaccineMui(ma_dangky, maVaccineList, maComboList);

                        MessageBox.Show("Lập hóa đơn thành công!");
                        frm_HienThiHoaDon frm = new frm_HienThiHoaDon(ma_hoadon, tenDangNhap);
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Hủy lập hóa đơn.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lập hóa đơn: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để lập hóa đơn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            KiemTraCanhBaoVaccine();
        }
        // Hàm tạo mã hóa đơn mới
        private string GenerateMaHoaDon()
        {
            try
            {
                string ma_hoadon = "";

                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    // Truy vấn để lấy mã hóa đơn cuối cùng
                    string query = "SELECT TOP 1 ma_hoadon FROM HOADON ORDER BY ma_hoadon DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            string lastMaHoaDon = result.ToString();
                            string prefix = lastMaHoaDon.Substring(0, 2); // Lấy phần đầu của mã hóa đơn (VD: "HD")
                            int number = int.Parse(lastMaHoaDon.Substring(2)) + 1; // Lấy phần số đếm và tăng lên 1

                            ma_hoadon = $"{prefix}{number:000}"; // Tạo mã hóa đơn mới
                        }
                        else
                        {
                            ma_hoadon = "HD001"; // Nếu không có hóa đơn nào thì sử dụng mã mặc định
                        }
                    }
                }

                return ma_hoadon;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo mã hóa đơn mới: " + ex.Message);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value.Date;

            // Chuẩn bị câu truy vấn SQL
            string queryString = @"
        SELECT DISTINCT 
            nd.ma_dangky,
            nd.hoten_nguoitiem,
            nd.ngaysinh_nguoitiem,
            nd.gioitinh_nguoitiem,
            nd.diachi_nguoitiem,
            nd.hoten_nguoilienhe,
            nd.sdt_nguoilienhe,
            nd.ma_khachhang,
            kh.hoten_khachhang,
            nd.phi_luukho,
            nd.tongthanhtoan,
            nd.ngay_dangky,
            nd.ngay_muontiem,
            nd.ma_voucher
        FROM 
            NGUOITIEM_DANGKY nd
        LEFT JOIN 
            KHACHHANG kh ON nd.ma_khachhang = kh.ma_khachhang
        LEFT JOIN 
            NGUOITIEM_MUAVACCINE nmv ON nd.ma_dangky = nmv.ma_dangky
        LEFT JOIN 
            VACCINE vc ON nmv.ma_vaccine = vc.ma_vaccine
        LEFT JOIN 
            NGUOITIEM_MUACOMBO nmc ON nd.ma_dangky = nmc.ma_dangky
        LEFT JOIN 
            COMBO_VACCINE cb ON nmc.ma_combo = cb.ma_combo
        WHERE 
            CONVERT(date, nd.ngay_dangky) = @selectedDate";

            // Thực hiện truy vấn SQL và hiển thị kết quả trong DataGridView
            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string maDangKy = textBox1.Text.Trim();

            // Chuẩn bị câu truy vấn SQL
            string queryString = @"
        SELECT DISTINCT 
            nd.ma_dangky,
            nd.hoten_nguoitiem,
            nd.ngaysinh_nguoitiem,
            nd.gioitinh_nguoitiem,
            nd.diachi_nguoitiem,
            nd.hoten_nguoilienhe,
            nd.sdt_nguoilienhe,
            nd.ma_khachhang,
            kh.hoten_khachhang,
            nd.phi_luukho,
            nd.tongthanhtoan,
            nd.ngay_dangky,
            nd.ngay_muontiem,
            nd.ma_voucher
        FROM 
            NGUOITIEM_DANGKY nd
        LEFT JOIN 
            KHACHHANG kh ON nd.ma_khachhang = kh.ma_khachhang
        LEFT JOIN 
            NGUOITIEM_MUAVACCINE nmv ON nd.ma_dangky = nmv.ma_dangky
        LEFT JOIN 
            VACCINE vc ON nmv.ma_vaccine = vc.ma_vaccine
        LEFT JOIN 
            NGUOITIEM_MUACOMBO nmc ON nd.ma_dangky = nmc.ma_dangky
        LEFT JOIN 
            COMBO_VACCINE cb ON nmc.ma_combo = cb.ma_combo
        WHERE 
            nd.ma_dangky = @maDangKy";

            // Thực hiện truy vấn SQL và hiển thị kết quả trong DataGridView
            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@maDangKy", maDangKy);
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void KiemTraCanhBaoVaccine()
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_GetLowStockVaccines", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Kiểm tra và hiển thị thông báo
                foreach (DataRow row in dt.Rows)
                {
                    int soLuongVaccine = Convert.ToInt32(row["Số Lượng Vaccine"]);
                    if (soLuongVaccine < 10)
                    {
                        MessageBox.Show($"Vaccine '{row["Tên Vaccine"]}' còn số lượng ít ({soLuongVaccine}). Vui lòng nhập thêm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra cảnh báo Vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btndschualaphoadon_Click(object sender, EventArgs e)
        {
            try
            {
                // Kết nối CSDL
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                // Truy vấn danh sách các mã đăng ký chưa có hóa đơn và thông tin tương ứng
                string query = @"SELECT ND.ma_dangky, ND.hoten_nguoitiem, ND.ngaysinh_nguoitiem, ND.gioitinh_nguoitiem, ND.diachi_nguoitiem, 
                           ND.hoten_nguoilienhe, ND.sdt_nguoilienhe, KH.hoten_khachhang, ND.phi_luukho, ND.tongthanhtoan, 
                           ND.ngay_dangky, ND.ngay_muontiem, ND.ma_voucher, ND.ma_khachhang
                    FROM NGUOITIEM_DANGKY ND
                    LEFT JOIN KHACHHANG KH ON ND.ma_khachhang = KH.ma_khachhang
                    LEFT JOIN HOADON HD ON ND.ma_dangky = HD.ma_dangky
                    WHERE HD.ma_dangky IS NULL";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                // Tạo DataTable để lưu kết quả truy vấn
                DataTable dt = new DataTable();
                dt.Load(reader);

                // Hiển thị kết quả trong DataGridView
                dataGridView1.DataSource = dt;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị danh sách mã đăng ký chưa được lập hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //insert tiemvaccine_mui
        private void InsertTiemVaccineMui(string maDangKy, List<string> maVaccineList, List<string> maComboList)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();

                // Duyệt qua từng mã vaccine trong maVaccineList
                foreach (string maVaccine in maVaccineList)
                {
                    // Kiểm tra và lấy số mũi vaccine hiện tại
                    int muiVaccine = GetMaxMuiVaccine(connection, maDangKy, maVaccine);

                    // Thêm vào bảng TIEMVACCINE_MUI
                    InsertMuiVaccine(connection, maDangKy, maVaccine, muiVaccine, null);
                }

                // Duyệt qua từng mã combo trong maComboList
                foreach (string maCombo in maComboList)
                {
                    // Lấy danh sách mã vaccine trong combo từ bảng CHITIET_COMBO_VACCXINE
                    List<string> vaccinesInCombo = GetVaccinesInCombo(connection, maCombo);

                    // Duyệt qua từng mã vaccine trong combo và thêm từng mũi vaccine vào bảng TIEMVACCINE_MUI
                    foreach (string maVaccine in vaccinesInCombo)
                    {
                        // Lấy số lượng vaccine cần thêm từ CHITIET_COMBO_VACCXINE
                        int soLuong = GetSoLuongVaccine(connection, maCombo, maVaccine);

                        // Thêm từng mũi vaccine vào bảng TIEMVACCINE_MUI
                        for (int i = 1; i <= soLuong; i++)
                        {
                            // Kiểm tra và lấy số mũi vaccine hiện tại
                            int muiVaccine = GetMaxMuiVaccine(connection, maDangKy, maVaccine);

                            // Thêm vào bảng TIEMVACCINE_MUI với mã combo
                            InsertMuiVaccine(connection, maDangKy, maVaccine, muiVaccine, maCombo);
                        }
                    }
                }
            }
        }


        private int GetSoLuongVaccine(SqlConnection connection, string maCombo, string maVaccine)
        {
            int soLuong = 0;

            // Lấy số lượng vaccine từ CHITIET_COMBO_VACCXINE
            string query = @"
        SELECT soluong_vaccine
        FROM CHITIET_COMBO_VACCXINE
        WHERE ma_combo = @maCombo AND ma_vaccine = @maVaccine";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCombo", maCombo);
                command.Parameters.AddWithValue("@maVaccine", maVaccine);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    soLuong = Convert.ToInt32(result);
                }
            }

            return soLuong;
        }


        private int GetMaxMuiVaccine(SqlConnection connection, string maDangKy, string maVaccine)
        {
            int muiVaccine = 1;

            // Kiểm tra số mũi vaccine hiện tại cho mã đăng ký và mã vaccine
            string checkQuery = @"
        SELECT MAX(mui_vaccine)
        FROM TIEMVACCINE_MUI
        WHERE ma_dangky = @maDangKy AND ma_vaccine = @maVaccine";

            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                checkCommand.Parameters.AddWithValue("@maVaccine", maVaccine);

                object result = checkCommand.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    muiVaccine = Convert.ToInt32(result) + 1;
                }
            }

            return muiVaccine;
        }

        private List<string> GetVaccinesInCombo(SqlConnection connection, string maCombo)
        {
            List<string> vaccines = new List<string>();

            // Lấy danh sách mã vaccine trong combo từ bảng CHITIET_COMBO_VACCXINE
            string query = @"
        SELECT ma_vaccine
        FROM CHITIET_COMBO_VACCXINE
        WHERE ma_combo = @maCombo";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maCombo", maCombo);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vaccines.Add(reader["ma_vaccine"].ToString());
                    }
                }
            }

            return vaccines;
        }

        private void InsertMuiVaccine(SqlConnection connection, string maDangKy, string maVaccine, int muiVaccine, string maCombo)
        {
            // Thêm vào bảng TIEMVACCINE_MUI
            string insertQuery = @"
        INSERT INTO TIEMVACCINE_MUI (ma_dangky, ma_vaccine, mui_vaccine, da_tiem, ma_combo)
        VALUES (@maDangKy, @maVaccine, @muiVaccine, 0, @maCombo)";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                insertCommand.Parameters.AddWithValue("@maVaccine", maVaccine);
                insertCommand.Parameters.AddWithValue("@muiVaccine", muiVaccine);
                insertCommand.Parameters.AddWithValue("@maCombo", (object)maCombo ?? DBNull.Value);
                insertCommand.ExecuteNonQuery();
            }
        }
    }
}
