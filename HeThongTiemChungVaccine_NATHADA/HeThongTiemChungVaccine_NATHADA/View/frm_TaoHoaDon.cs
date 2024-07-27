using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_TaoHoaDon : Form
    {
        private bool isComboSelected = false;
        private string selectedComboID = "";
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        private string maNhanVien;
        private string tenDangNhap;
        private string maDangKy;
        Control_NhanVien controlNhanVien = new Control_NhanVien();
        public frm_TaoHoaDon(string tenDangNhap)
        {
            this.maNhanVien = controlNhanVien.LayMaNhanVienTuTenDangNhap(tenDangNhap);
            this.tenDangNhap = tenDangNhap;
            InitializeComponent();
            cbbsanpham.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbgioitinh.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbmoiquanhe.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadDataToComboBox();
            LoadGioiTinhComboBox();
            ComboBoxMoiQuanHe();
            DataGridViewTextBoxColumn tenSanPhamColumn = new DataGridViewTextBoxColumn();
            tenSanPhamColumn.HeaderText = "Tên sản phẩm";
            tenSanPhamColumn.Name = "TenSanPham"; 
            dataGridView1.Columns.Add(tenSanPhamColumn);

            // Thêm cột "Giá sản phẩm"
            DataGridViewTextBoxColumn giaSanPhamColumn = new DataGridViewTextBoxColumn();
            giaSanPhamColumn.HeaderText = "Giá sản phẩm";
            giaSanPhamColumn.Name = "GiaSanPham"; 
            dataGridView1.Columns.Add(giaSanPhamColumn);

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsValidData())
            {
                if (!KiemTraSoLuongSanPham())
                {
                    return;
                }

                if (!KiemTraLichSuMuaHang())
                {
                    return;
                }

                if (!KiemTraCanhBaoVaccine())
                {
                    return;
                }

                // Kiểm tra nếu là combo thì gọi CanSellCombo
                if (isComboSelected)
                {
                    if (!CanSellCombo(selectedComboID))
                    {
                        return;
                    }
                }

                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn lập hóa đơn không?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    InsertData();
                    InsertTiemVaccineMui(maDangKy);
                }
            }
        }
      

        private void cbbsanpham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbsanpham.SelectedItem != null)
            {
                string selectedProduct = cbbsanpham.SelectedItem.ToString();

                // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm được chọn
                string query = "";
                string tableName = "";

                // Kiểm tra xem sản phẩm là vaccine hay combo
                if (selectedProduct.StartsWith("V")) // Nếu mã sản phẩm bắt đầu bằng "V", tức là là vaccine
                {
                    tableName = "VACCINE"; // Tên bảng chứa thông tin vaccine
                    query = $"SELECT ten_vaccine, gia_vacine FROM {tableName} WHERE ten_vaccine = @selectedProduct";
                    isComboSelected = false; // Không phải combo
                    selectedComboID = "";
                }
                else // Nếu không, là combo
                {
                    tableName = "COMBO_VACCINE"; // Tên bảng chứa thông tin combo
                    query = $"SELECT ten_combo, gia_combo FROM {tableName} WHERE ten_combo = @selectedProduct";
                    isComboSelected = true; // Là combo
                    selectedComboID = selectedProduct;
                }

                // Thực hiện truy vấn và lấy dữ liệu
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@selectedProduct", selectedProduct);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // Lấy thông tin sản phẩm
                            string tenSanPham = reader.GetString(0); // Lấy tên sản phẩm

                            // Kiểm tra nếu giá sản phẩm không phải là DBNull
                            float giaSanPham = 0;
                            if (!reader.IsDBNull(1))
                            {
                                // Nếu giá sản phẩm không phải là DBNull, thử lấy dữ liệu dưới dạng double
                                if (reader.GetFieldType(1) == typeof(double))
                                {
                                    giaSanPham = (float)reader.GetDouble(1); // Ép kiểu double sang float
                                }
                                else if (reader.GetFieldType(1) == typeof(decimal))
                                {
                                    giaSanPham = (float)reader.GetDecimal(1); // Ép kiểu decimal sang float
                                }
                            }

                            // Kiểm tra xem sản phẩm đã tồn tại trong DataGridView hay chưa
                            bool exists = false;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells["TenSanPham"].Value.ToString() == tenSanPham)
                                {
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                // Thêm thông tin sản phẩm vào DataGridView
                                DataGridViewRow row = new DataGridViewRow();
                                row.CreateCells(dataGridView1);
                                row.Cells[0].Value = tenSanPham; // Thêm tên sản phẩm vào cột 0
                                row.Cells[1].Value = giaSanPham; // Thêm giá sản phẩm vào cột 1
                                dataGridView1.Rows.Add(row);

                                // Tính toán tổng thanh toán và phí lưu kho
                                UpdateTotals();

                                MessageBox.Show("Bạn đã chọn: " + selectedProduct);
                            }
                            else
                            {
                                MessageBox.Show("Sản phẩm đã tồn tại trong danh sách.");
                            }
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải dữ liệu từ cơ sở dữ liệu: " + ex.Message);
                    }
                }
            }
        }

        private void UpdateTotals()
        {
            float phiLuuKho = 0;
            float tongThanhToan = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                float giaSanPham = (float)row.Cells["GiaSanPham"].Value;

                if (row.Cells["TenSanPham"].Value.ToString().StartsWith("V"))
                {
                    // Nếu là vaccine, không tính phí lưu kho
                    tongThanhToan += giaSanPham;
                }
                else
                {
                    // Nếu là combo, tính phí lưu kho
                    float phiCombo = giaSanPham * 0.2f; // Phí lưu kho là 20% giá combo
                    phiLuuKho += phiCombo;
                    tongThanhToan += giaSanPham + phiCombo;
                }
            }

            // Gán giá trị vào các ô textbox
            txtphiluukho.Text = phiLuuKho.ToString("#,0") + "₫";
            txttongthanhtoan.Text = tongThanhToan.ToString("#,0") + "₫";
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Xóa dòng được chọn
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);

                // Tính toán lại tổng thanh toán và phí lưu kho
                float phiLuuKho = 0;
                float tongThanhToan = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        string tenSanPham = row.Cells[0].Value.ToString();
                        float giaSanPham = Convert.ToSingle(row.Cells[1].Value);

                        if (tenSanPham.StartsWith("V"))
                        {
                            // Nếu sản phẩm là vaccine, chỉ cộng giá vào tổng thanh toán
                            tongThanhToan += giaSanPham;
                        }
                        else
                        {
                            // Nếu sản phẩm là combo, tính phí lưu kho và cộng cả hai vào tổng thanh toán
                            float phiCombo = giaSanPham * 0.2f;
                            phiLuuKho += phiCombo;
                            tongThanhToan += giaSanPham + phiCombo;
                        }
                    }
                }

                // Cập nhật giá trị mới vào các ô textbox
                txtphiluukho.Text = phiLuuKho.ToString("#,0") + "₫";
                txttongthanhtoan.Text = tongThanhToan.ToString("#,0") + "₫";
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để gỡ bỏ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private string GenerateNewMaDangKy()
        {
            string newMaDangKy = "DK001";
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT TOP 1 ma_dangky FROM NGUOITIEM_DANGKY ORDER BY ma_dangky DESC";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string lastMaDangKy = reader.GetString(0);
                        int lastNumber = int.Parse(lastMaDangKy.Substring(2));
                        newMaDangKy = "DK" + (lastNumber + 1).ToString("D3");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo mã đăng ký mới: " + ex.Message);
                }
            }
            return newMaDangKy;
        }
        private string GenerateNewMaHoaDon()
        {
            string newMaDangKy = "HD001";
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT TOP 1 ma_hoadon FROM HOADON ORDER BY ma_hoadon DESC";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string lastMaDangKy = reader.GetString(0);
                        int lastNumber = int.Parse(lastMaDangKy.Substring(2));
                        newMaDangKy = "HD" + (lastNumber + 1).ToString("D3");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo mã đăng ký mới: " + ex.Message);
                }
            }
            return newMaDangKy;
        }
        private bool IsValidData()
        {
            // Kiểm tra txttennguoitiem
            if (string.IsNullOrWhiteSpace(txttennguoitiem.Text) || txttennguoitiem.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo tên người tiêm đúng định dạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra dtpngaysinhnguoitiem
            if (dtpngaysinhnguoitiem.Value.Date >= DateTime.Today)
            {
                MessageBox.Show("Ngày sinh người tiêm phải nhỏ hơn ngày hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (dtpngaytiem.Value.Date <= DateTime.Today)
            {
                MessageBox.Show("Ngày tiêm tiêm phải lơn hơn ngày hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra cbbgioitinh
            if (string.IsNullOrWhiteSpace(cbbgioitinh.Text) || (cbbgioitinh.Text != "Nam" && cbbgioitinh.Text != "Nữ"))
            {
                MessageBox.Show("Vui lòng chọn giới tính là Nam hoặc Nữ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra txtdiachi
            if (string.IsNullOrWhiteSpace(txtdiachi.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo địa chỉ đúng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra txtnguoilienhe
            if (string.IsNullOrWhiteSpace(txtnguoilienhe.Text) || txtnguoilienhe.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo tên người liên hệ đúng định dạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra txtsodienthoai
            if (string.IsNullOrWhiteSpace(txtsodienthoai.Text) || !txtsodienthoai.Text.StartsWith("0") || txtsodienthoai.Text.Length != 10 || !txtsodienthoai.Text.All(char.IsDigit))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo số điện thoại bắt đầu bằng số 0 và có đủ 10 số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Kiểm tra txtsodienthoaikhachhang
            if (string.IsNullOrWhiteSpace(txtsodienthoaikhachhang.Text) || !txtsodienthoaikhachhang.Text.StartsWith("0") || txtsodienthoaikhachhang.Text.Length != 10 || !txtsodienthoaikhachhang.Text.All(char.IsDigit))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo số điện thoại khách hàng bắt đầu bằng số 0 và có đủ 10 số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Nếu đã vượt qua tất cả các kiểm tra, trả về true
            return true;
        }
        private void LoadDataToComboBox()
        {
            //try
            //{
            //    // Mở kết nối đến cơ sở dữ liệu trước khi thực hiện truy vấn
            //    SqlConnection connection = connect.KetNoiCSDL();
            //    connection.Open();

            //    string query = "SELECT ten_vaccine FROM VACCINE " +
            //                   "UNION " +
            //                   "SELECT ten_combo FROM COMBO_VACCINE";

            //    SqlCommand command = new SqlCommand(query, connection);
            //    SqlDataReader reader = command.ExecuteReader();

            //    List<string> items = new List<string>();

            //    while (reader.Read())
            //    {
            //        items.Add(reader["ten_vaccine"].ToString());
            //    }
            //    reader.Close();

            //    // Đóng kết nối
            //    connection.Close();

            //    // Thêm các mục vào ComboBox
            //    cbbsanpham.Items.AddRange(items.ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Lỗi khi tải dữ liệu vào ComboBox: " + ex.Message);
            //}
            try
            {
                // Mở kết nối đến cơ sở dữ liệu trước khi thực hiện truy vấn
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();

                // Truy vấn để lấy tên vaccine có trong kho và tên combo
                string query = @"
            SELECT v.ten_vaccine 
            FROM VACCINE v
            INNER JOIN KHO k ON v.ma_vaccine = k.ma_vaccine
            UNION 
            SELECT ten_combo 
            FROM COMBO_VACCINE";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                List<string> items = new List<string>();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0)); // Lấy tên vaccine hoặc tên combo
                }
                reader.Close();

                // Đóng kết nối
                connection.Close();

                // Thêm các mục vào ComboBox
                cbbsanpham.Items.AddRange(items.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu vào ComboBox: " + ex.Message);
            }

        }

        private void LoadGioiTinhComboBox()
        {
            cbbgioitinh.Items.Clear();

            cbbgioitinh.Items.Add("Nam");
            cbbgioitinh.Items.Add("Nữ");
        }
        private void ComboBoxMoiQuanHe()
        {
            cbbmoiquanhe.Items.Clear();

            cbbmoiquanhe.Items.Add("Bản thân");
            cbbmoiquanhe.Items.Add("Con");

            cbbmoiquanhe.Items.Add("Cha");
            cbbmoiquanhe.Items.Add("Mẹ");

            cbbmoiquanhe.Items.Add("Vợ");
            cbbmoiquanhe.Items.Add("Chồng");

            cbbmoiquanhe.Items.Add("Anh");
            cbbmoiquanhe.Items.Add("Chị");

            cbbmoiquanhe.Items.Add("Em trai");
            cbbmoiquanhe.Items.Add("Em gái");
            cbbmoiquanhe.Items.Add("Ông");
            cbbmoiquanhe.Items.Add("Bà");
            cbbmoiquanhe.Items.Add("Họ hàng");
            cbbmoiquanhe.Items.Add("Khác");
        }
        private void InsertData()
        {
            maDangKy = GenerateNewMaDangKy();
            string hotenNguoiTiem = txttennguoitiem.Text;
            DateTime ngaysinhNguoiTiem = dtpngaysinhnguoitiem.Value;
            string gioitinhNguoiTiem = cbbgioitinh.SelectedItem.ToString();
            string diachiNguoiTiem = txtdiachi.Text;
            string hotenNguoiLienHe = txtnguoilienhe.Text;
            string moiQuanHeNguoiTiem = cbbmoiquanhe.SelectedItem.ToString();
            int sdtNguoiLienHe = int.Parse(txtsodienthoai.Text);
            string sdtKhachHang = txtsodienthoaikhachhang.Text;
            string maKhachHang = GetMaKhachHangBySdt(sdtKhachHang);
            float phiLuuKho;
            float tongThanhToan;

            // Loại bỏ dấu phẩy và ký hiệu tiền tệ từ chuỗi
            string phiLuuKhoText = txtphiluukho.Text.Replace(",", "").Replace("₫", "");
            string tongThanhToanText = txttongthanhtoan.Text.Replace(",", "").Replace("₫", "");

            // Kiểm tra và chuyển đổi giá trị từ textbox sang kiểu float an toàn
            if (!float.TryParse(phiLuuKhoText, out phiLuuKho))
            {
                MessageBox.Show("Giá trị phí lưu kho không hợp lệ!");
                return; // Thoát khỏi hàm nếu không thể chuyển đổi được
            }

            // Kiểm tra và chuyển đổi giá trị từ textbox sang kiểu float an toàn
            if (!float.TryParse(tongThanhToanText, out tongThanhToan))
            {
                MessageBox.Show("Giá trị tổng thanh toán không hợp lệ!");
                return; // Thoát khỏi hàm nếu không thể chuyển đổi được
            }


            DateTime ngayDangKy = DateTime.Now; // Lấy ngày hiện tại
            DateTime ngayMuonTiem = dtpngaytiem.Value;

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "INSERT INTO NGUOITIEM_DANGKY (ma_dangky, hoten_nguoitiem, ngaysinh_nguoitiem, gioitinh_nguoitiem, diachi_nguoitiem, hoten_nguoilienhe, moiquanhe_nguoitiem, sdt_nguoilienhe, ma_khachhang, phi_luukho, tongthanhtoan, ngay_dangky, ngay_muontiem) VALUES (@ma_dangky, @hoten_nguoitiem, @ngaysinh_nguoitiem, @gioitinh_nguoitiem, @diachi_nguoitiem, @hoten_nguoilienhe, @moiquanhe_nguoitiem, @sdt_nguoilienhe, @ma_khachhang, @phi_luukho, @tongthanhtoan, @ngay_dangky, @ngay_muontiem)";

                command.Parameters.AddWithValue("@ma_dangky", maDangKy);
                command.Parameters.AddWithValue("@hoten_nguoitiem", hotenNguoiTiem);
                command.Parameters.AddWithValue("@ngaysinh_nguoitiem", ngaysinhNguoiTiem);
                command.Parameters.AddWithValue("@gioitinh_nguoitiem", gioitinhNguoiTiem);
                command.Parameters.AddWithValue("@diachi_nguoitiem", diachiNguoiTiem);
                command.Parameters.AddWithValue("@hoten_nguoilienhe", hotenNguoiLienHe);
                command.Parameters.AddWithValue("@moiquanhe_nguoitiem", moiQuanHeNguoiTiem);
                command.Parameters.AddWithValue("@sdt_nguoilienhe", sdtNguoiLienHe);
                command.Parameters.AddWithValue("@ma_khachhang", maKhachHang);
                command.Parameters.AddWithValue("@phi_luukho", phiLuuKho);
                command.Parameters.AddWithValue("@tongthanhtoan", tongThanhToan);
                command.Parameters.AddWithValue("@ngay_dangky", ngayDangKy);
                command.Parameters.AddWithValue("@ngay_muontiem", ngayMuonTiem);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    // Insert dữ liệu vào bảng NGUOITIEM_MUAVACCINE và NGUOITIEM_MUACOMBO
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["TenSanPham"].Value != null)
                        {
                            string tenSanPham = row.Cells["TenSanPham"].Value.ToString();
                            float giaSanPham = float.Parse(row.Cells["GiaSanPham"].Value.ToString());

                            // Kiểm tra nếu sản phẩm là vaccine hay combo
                            if (tenSanPham.StartsWith("V"))
                            {
                                // Lấy mã vaccine từ bảng VACCINE
                                SqlCommand vaccineCommand = new SqlCommand("SELECT ma_vaccine FROM VACCINE WHERE ten_vaccine = @tenSanPham", connection);
                                vaccineCommand.Parameters.AddWithValue("@tenSanPham", tenSanPham);
                                string maVaccine = (string)vaccineCommand.ExecuteScalar();

                                // Insert vào bảng NGUOITIEM_MUAVACCINE
                                SqlCommand insertVaccineCommand = new SqlCommand("INSERT INTO NGUOITIEM_MUAVACCINE (ma_dangky, ma_vaccine, so_luong) VALUES (@ma_dangky, @ma_vaccine, @so_luong)", connection);
                                insertVaccineCommand.Parameters.AddWithValue("@ma_dangky", maDangKy);
                                insertVaccineCommand.Parameters.AddWithValue("@ma_vaccine", maVaccine);
                                insertVaccineCommand.Parameters.AddWithValue("@so_luong", 1); // Giả sử số lượng là 1
                                insertVaccineCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                // Lấy mã combo từ bảng COMBO_VACCINE
                                SqlCommand comboCommand = new SqlCommand("SELECT ma_combo FROM COMBO_VACCINE WHERE ten_combo = @tenSanPham", connection);
                                comboCommand.Parameters.AddWithValue("@tenSanPham", tenSanPham);
                                string maCombo = (string)comboCommand.ExecuteScalar();

                                // Insert vào bảng NGUOITIEM_MUACOMBO
                                SqlCommand insertComboCommand = new SqlCommand("INSERT INTO NGUOITIEM_MUACOMBO (ma_dangky, ma_combo, so_luong) VALUES (@ma_dangky, @ma_combo, @so_luong)", connection);
                                insertComboCommand.Parameters.AddWithValue("@ma_dangky", maDangKy);
                                insertComboCommand.Parameters.AddWithValue("@ma_combo", maCombo);
                                insertComboCommand.Parameters.AddWithValue("@so_luong", 1); // Giả sử số lượng là 1
                                insertComboCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    InsertHoaDonAndChiTietHoaDon(maDangKy);
                    InsertTiemVaccineMui(maDangKy);
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đăng ký tiêm vaccine: " + ex.Message);
                }
            }
        }

        private string GetMaKhachHangBySdt(string sdtKhachHang)
        {
            string maKhachHang = string.Empty;
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT ma_khachhang FROM KHACHHANG WHERE sdt_khachhang = @sdt_khachhang";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@sdt_khachhang", sdtKhachHang);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        maKhachHang = reader.GetString(0);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm mã khách hàng: " + ex.Message);
                }
            }
            return maKhachHang;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private string LayHoTenNhanVienTuTenDangNhap(string tenDangNhap)
        {
            string hoTen = "";

            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                // Mở kết nối
                connection.Open();

                // Tạo command để truy vấn dữ liệu
                using (SqlCommand command = new SqlCommand("SELECT NV.hoten_nhanvien FROM NHANVIEN NV JOIN TAIKHOAN TK ON NV.ma_nhanvien = TK.ma_nhanvien WHERE TK.tendangnhap = @tenDangNhap", connection))
                {
                    // Thêm tham số cho câu truy vấn
                    command.Parameters.AddWithValue("@tenDangNhap", tenDangNhap);

                    // Thực thi truy vấn và gán kết quả cho biến hoTen
                    hoTen = (string)command.ExecuteScalar();
                }
            }

            return hoTen;
        }
        private Tuple<List<string>, List<string>> LayMaVaccineVaComboTuMaDangKy(string maDangKy)
        {
            List<string> maVaccines = new List<string>();
            List<string> maCombos = new List<string>();

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    // Lấy mã vaccine từ bảng NGUOITIEM_MUAVACCINE
                    command.CommandText = "SELECT ma_vaccine FROM NGUOITIEM_MUAVACCINE WHERE ma_dangky = @ma_dangky_vaccine";
                    command.Parameters.AddWithValue("@ma_dangky_vaccine", maDangKy);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string maVaccine = reader["ma_vaccine"].ToString();
                            maVaccines.Add(maVaccine);
                        }
                    }

                    // Lấy mã combo từ bảng NGUOITIEM_MUACOMBO
                    command.CommandText = "SELECT ma_combo FROM NGUOITIEM_MUACOMBO WHERE ma_dangky = @ma_dangky_combo";
                    command.Parameters.AddWithValue("@ma_dangky_combo", maDangKy);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string maCombo = reader["ma_combo"].ToString();
                            maCombos.Add(maCombo);
                        }
                    }
                }
            }

            return Tuple.Create(maVaccines, maCombos);
        }

        private void InsertHoaDonAndChiTietHoaDon(string maDangKy)
        {
            string maHoaDon = GenerateNewMaHoaDon();

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    try
                    {
                        Tuple<List<string>, List<string>> maVaccineVaCombo = LayMaVaccineVaComboTuMaDangKy(maDangKy);
                        List<string> maVaccines = maVaccineVaCombo.Item1;
                        List<string> maCombos = maVaccineVaCombo.Item2;

                        // Tạo chuỗi chứa tất cả các mã combo, phân tách bằng dấu phẩy
                        string combinedCombo = string.Join(", ", maCombos);

                        // Tạo chuỗi chứa tất cả các mã vaccine, phân tách bằng dấu phẩy
                        string combinedVaccine = string.Join(", ", maVaccines);

                        // Insert vào bảng HOADON
                        command.CommandText = "INSERT INTO HOADON (ma_hoadon, ma_dangky, ma_nhanvien, ma_khachhang, ma_voucher, ma_combo, trangthai) VALUES (@ma_hoadon, @ma_dangky, @ma_nhanvien, @ma_khachhang, @ma_voucher, @ma_combo, @trangthai)";
                        command.Parameters.AddWithValue("@ma_hoadon", maHoaDon);
                        command.Parameters.AddWithValue("@ma_dangky", maDangKy);
                        command.Parameters.AddWithValue("@ma_nhanvien", maNhanVien);
                        command.Parameters.AddWithValue("@ma_khachhang", GetMaKhachHangBySdt(txtsodienthoaikhachhang.Text));
                        command.Parameters.AddWithValue("@ma_voucher", DBNull.Value);
                        command.Parameters.AddWithValue("@ma_combo", combinedCombo); // Sử dụng chuỗi chứa các mã combo
                        command.Parameters.AddWithValue("@trangthai", 1);
                        command.ExecuteNonQuery();

                        // Insert vào bảng CHITIET_HOADON
                        command.CommandText = "INSERT INTO CHITIET_HOADON (ma_hoadon, ma_vaccine, soluong_vaccine, chietkhau, thanhtien, thoigian_thanhtoan, hinhthuc_thanhtoan) VALUES (@ma_hoadon, @ma_vaccine, @soluong_vaccine, @chietkhau, @thanhtien, @thoigian_thanhtoan, @hinhthuc_thanhtoan)";
                        command.Parameters.Clear();

                        // Chèn dữ liệu vào bảng CHITIET_HOADON với chuỗi các mã vaccine
                        command.Parameters.AddWithValue("@ma_hoadon", maHoaDon);
                        command.Parameters.AddWithValue("@ma_vaccine", combinedVaccine); // Sử dụng chuỗi chứa các mã vaccine
                        command.Parameters.AddWithValue("@soluong_vaccine", maVaccines.Count); // Tổng số lượng vaccine
                        command.Parameters.AddWithValue("@chietkhau", 0);
                        command.Parameters.AddWithValue("@thanhtien", 0);
                        command.Parameters.AddWithValue("@thoigian_thanhtoan", DateTime.Now);
                        command.Parameters.AddWithValue("@hinhthuc_thanhtoan", "Thanh toán tại chỗ");
                        command.ExecuteNonQuery();

                        // Commit transaction
                        transaction.Commit();
                        MessageBox.Show("Đăng ký tiêm vaccine thành công!");
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi xảy ra
                        transaction.Rollback();
                        MessageBox.Show("Lỗi khi đăng ký tiêm vaccine: " + ex.Message);
                    }
                }
            }
            frm_HienThiHoaDon_OFF frm = new frm_HienThiHoaDon_OFF(maHoaDon, tenDangNhap);
            frm.ShowDialog();
        }
        public void clear()
        {
            txttennguoitiem.Text = "";
            cbbgioitinh.SelectedIndex = -1; // Chọn một giá trị mặc định nếu có
            txtdiachi.Text = "";
            dataGridView1.Rows.Clear(); // Xóa tất cả các dòng trong DataGridView
            txtnguoilienhe.Text = "";
            txtsodienthoai.Text = "";
            cbbmoiquanhe.SelectedIndex = -1; // Chọn một giá trị mặc định nếu có
            txtsodienthoaikhachhang.Text = "";
            cbbsanpham.SelectedIndex = -1; // Chọn một giá trị mặc định nếu có
            txtphiluukho.Text = "";
            txttongthanhtoan.Text = "";
        }

        private void InsertTiemVaccineMui(string maDangKy)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["TenSanPham"].Value != null)
                    {
                        string tenSanPham = row.Cells["TenSanPham"].Value.ToString();
                        string maVaccine = "";
                        string maCombo = "";
                        int muiVaccine = 1; // Giá trị mặc định của mũi vaccine mới

                        if (tenSanPham.StartsWith("V")) // Nếu là vaccine
                        {
                            // Lấy mã vaccine từ bảng VACCINE
                            using (SqlCommand command = new SqlCommand("SELECT ma_vaccine FROM VACCINE WHERE ten_vaccine = @tenSanPham", connection))
                            {
                                command.Parameters.AddWithValue("@tenSanPham", tenSanPham);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        maVaccine = reader["ma_vaccine"].ToString();
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(maVaccine))
                            {
                                // Sử dụng kết nối khác để kiểm tra số thứ tự của mũi vaccine
                                using (SqlConnection checkConnection = connect.KetNoiCSDL())
                                {
                                    checkConnection.Open();
                                    using (SqlCommand checkCommand = new SqlCommand("SELECT MAX(mui_vaccine) FROM TIEMVACCINE_MUI WHERE ma_dangky = @maDangKy AND ma_vaccine = @maVaccine", checkConnection))
                                    {
                                        checkCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                                        checkCommand.Parameters.AddWithValue("@maVaccine", maVaccine);
                                        object result = checkCommand.ExecuteScalar();
                                        if (result != DBNull.Value)
                                        {
                                            muiVaccine = Convert.ToInt32(result) + 1;
                                        }
                                    }
                                }

                                // Thêm bản ghi mới vào bảng TIEMVACCINE_MUI
                                using (SqlCommand insertCommand = new SqlCommand("INSERT INTO TIEMVACCINE_MUI (ma_dangky, ma_vaccine, mui_vaccine, da_tiem, ma_combo) VALUES (@maDangKy, @maVaccine, @muiVaccine, 0, NULL)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                                    insertCommand.Parameters.AddWithValue("@maVaccine", maVaccine);
                                    insertCommand.Parameters.AddWithValue("@muiVaccine", muiVaccine);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (tenSanPham.StartsWith("C")) // Nếu là combo vaccine
                        {
                            // Lấy mã combo từ bảng COMBO_VACCINE
                            using (SqlCommand command = new SqlCommand("SELECT ma_combo FROM COMBO_VACCINE WHERE ten_combo = @tenSanPham", connection))
                            {
                                command.Parameters.AddWithValue("@tenSanPham", tenSanPham);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        maCombo = reader["ma_combo"].ToString();
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(maCombo))
                            {
                                // Lấy danh sách mã vaccine trong combo từ bảng CHITIET_COMBO_VACCXINE
                                List<string> vaccinesInCombo = GetVaccinesInCombo(connection, maCombo);

                                // Duyệt qua từng mã vaccine trong combo và thêm từng mũi vaccine vào bảng TIEMVACCINE_MUI
                                foreach (string vaccineInCombo in vaccinesInCombo)
                                {
                                    // Lấy số lượng mũi vaccine cần thêm từ CHITIET_COMBO_VACCXINE
                                    int soLuong = GetSoLuongVaccine(connection, maCombo, vaccineInCombo);

                                    // Thêm từng mũi vaccine vào bảng TIEMVACCINE_MUI
                                    for (int i = 1; i <= soLuong; i++)
                                    {
                                        // Kiểm tra và lấy số mũi vaccine hiện tại
                                        muiVaccine = GetMaxMuiVaccine(connection, maDangKy, vaccineInCombo);

                                        // Thêm vào bảng TIEMVACCINE_MUI với mã combo
                                        InsertMuiVaccine(connection, maDangKy, vaccineInCombo, muiVaccine, maCombo);
                                        muiVaccine++; // Tăng số mũi vaccine lên cho lần tiếp theo
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
        // Phương thức hỗ trợ: Lấy số lượng vaccine từ CHITIET_COMBO_VACCXINE
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

        // Phương thức hỗ trợ: Lấy số mũi vaccine hiện tại
        private int GetMaxMuiVaccine(SqlConnection connection, string maDangKy, string maVaccine)
        {
            int muiVaccine = 1;

            // Sử dụng kết nối khác để kiểm tra số thứ tự của mũi vaccine
            using (SqlConnection checkConnection = connect.KetNoiCSDL())
            {
                checkConnection.Open();
                using (SqlCommand checkCommand = new SqlCommand("SELECT MAX(mui_vaccine) FROM TIEMVACCINE_MUI WHERE ma_dangky = @maDangKy AND ma_vaccine = @maVaccine", checkConnection))
                {
                    checkCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                    checkCommand.Parameters.AddWithValue("@maVaccine", maVaccine);
                    object result = checkCommand.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        muiVaccine = Convert.ToInt32(result) + 1;
                    }
                }
            }

            return muiVaccine;
        }

        // Phương thức hỗ trợ: Thêm một mũi vaccine vào bảng TIEMVACCINE_MUI
        private void InsertMuiVaccine(SqlConnection connection, string maDangKy, string maVaccine, int muiVaccine, string maCombo)
        {
            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO TIEMVACCINE_MUI (ma_dangky, ma_vaccine, mui_vaccine, da_tiem, ma_combo) VALUES (@maDangKy, @maVaccine, @muiVaccine, 0, @maCombo)", connection))
            {
                insertCommand.Parameters.AddWithValue("@maDangKy", maDangKy);
                insertCommand.Parameters.AddWithValue("@maVaccine", maVaccine);
                insertCommand.Parameters.AddWithValue("@muiVaccine", muiVaccine);
                insertCommand.Parameters.AddWithValue("@maCombo", maCombo ?? (object)DBNull.Value);
                insertCommand.ExecuteNonQuery(); // Thực hiện lệnh chèn dữ liệu vào bảng TIEMVACCINE_MUI
            }
        }

        private bool KiemTraCanhBaoVaccine()
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
                    if (soLuongVaccine < 10 && soLuongVaccine > 0)
                    {
                        MessageBox.Show($"Vaccine '{row["Tên Vaccine"]}' còn số lượng ít ({soLuongVaccine}). Vui lòng nhập thêm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (soLuongVaccine <= 0)
                    {
                        MessageBox.Show($"Vaccine '{row["Tên Vaccine"]}' đã hết hàng. Không thể đăng ký mua.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false; // Trả về false nếu không đủ số lượng vaccine trong kho để bán combo
                    }
                }

                conn.Close();
                return true; // Trả về true nếu đủ số lượng vaccine trong kho để bán combo
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra cảnh báo Vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool KiemTraLichSuMuaHang()
        {
            string sdtKhachHang = txtsodienthoaikhachhang.Text;
            string maKhachHang = GetMaKhachHangBySdt(sdtKhachHang);
            string hotenNguoiTiem = txttennguoitiem.Text;

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Kiểm tra lịch sử mua vaccine và combo của người tiêm trong vòng 3 tháng và 9 tháng
                command.CommandText = @"
            SELECT 
                MAX(CASE WHEN ntm.so_luong = 1 THEN ndk.ngay_dangky ELSE NULL END) AS ngay_mua_vaccine,
                MAX(CASE WHEN ntc.so_luong = 1 THEN ndk.ngay_dangky ELSE NULL END) AS ngay_mua_combo
            FROM NGUOITIEM_DANGKY ndk
            LEFT JOIN NGUOITIEM_MUAVACCINE ntm ON ndk.ma_dangky = ntm.ma_dangky
            LEFT JOIN NGUOITIEM_MUACOMBO ntc ON ndk.ma_dangky = ntc.ma_dangky
            WHERE ndk.ma_khachhang = @ma_khachhang AND ndk.hoten_nguoitiem = @hoten_nguoitiem";

                command.Parameters.AddWithValue("@ma_khachhang", maKhachHang);
                command.Parameters.AddWithValue("@hoten_nguoitiem", hotenNguoiTiem);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    DateTime? ngayMuaVaccine = reader["ngay_mua_vaccine"] as DateTime?;
                    DateTime? ngayMuaCombo = reader["ngay_mua_combo"] as DateTime?;
                    DateTime ngayHienTai = DateTime.Now;

                    if (ngayMuaVaccine.HasValue && (ngayHienTai - ngayMuaVaccine.Value).TotalDays < 90)
                    {
                        MessageBox.Show("Người tiêm này đã mua vaccine trong vòng 3 tháng trước. Vui lòng đợi đủ 3 tháng để thực hiện mua vaccine tiếp.");
                        return false;
                    }

                    if (ngayMuaCombo.HasValue && (ngayHienTai - ngayMuaCombo.Value).TotalDays < 270)
                    {
                        MessageBox.Show("Người tiêm này đã mua combo trong vòng 9 tháng trước. Vui lòng đợi đủ 9 tháng để thực hiện mua combo tiếp.");
                        return false;
                    }
                }

                return true;
            }
        }
        private bool KiemTraSoLuongSanPham()
        {
            int soLuongVaccine = 0;
            int soLuongCombo = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["TenSanPham"].Value != null)
                {
                    string tenSanPham = row.Cells["TenSanPham"].Value.ToString();

                    if (tenSanPham.StartsWith("V"))
                    {
                        soLuongVaccine++;
                    }
                    else
                    {
                        soLuongCombo++;
                    }
                }
            }

            if (soLuongVaccine > 2)
            {
                MessageBox.Show("Bạn chỉ được phép mua tối đa 2 vaccine.");
                return false;
            }

            if (soLuongCombo > 1)
            {
                MessageBox.Show("Bạn chỉ được phép mua tối đa 1 combo.");
                return false;
            }

            if (soLuongVaccine > 0 && soLuongCombo > 0)
            {
                MessageBox.Show("Bạn chỉ được phép mua hoặc tối đa 2 vaccine hoặc tối đa 1 combo.");
                return false;
            }

            return true;
        }

        private bool CanSellCombo(string tenCombo)
        {
            try
            {
                using (SqlConnection conn = connect.KetNoiCSDL())
                {
                    conn.Open();

                    // Truy vấn để lấy mã combo từ tên combo
                    SqlCommand getMaComboCmd = new SqlCommand("SELECT ma_combo FROM COMBO_VACCINE WHERE ten_combo = @tenCombo", conn);
                    getMaComboCmd.Parameters.AddWithValue("@tenCombo", tenCombo);

                    string maCombo = getMaComboCmd.ExecuteScalar()?.ToString();

                    if (string.IsNullOrEmpty(maCombo))
                    {
                        MessageBox.Show("Không tìm thấy mã combo cho tên combo này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // Truy vấn chi tiết combo
                    SqlCommand cmd = new SqlCommand(
                        "SELECT ma_vaccine, soluong_vaccine FROM CHITIET_COMBO_VACCXINE WHERE ma_combo = @maCombo", conn);
                    cmd.Parameters.AddWithValue("@maCombo", maCombo);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtComboDetails = new DataTable();
                    da.Fill(dtComboDetails);


                    foreach (DataRow row in dtComboDetails.Rows)
                    {
                        string maVaccine = row["ma_vaccine"].ToString();
                        int soLuongVaccineTrongCombo = Convert.ToInt32(row["soluong_vaccine"]);

                        // Truy vấn số lượng vaccine trong kho
                        SqlCommand khoCmd = new SqlCommand(
                            "SELECT soluong_vaccine FROM KHO WHERE ma_vaccine = @maVaccine", conn);
                        khoCmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                        object result = khoCmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show($"Không tìm thấy vaccine '{maVaccine}' trong kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        int soLuongVaccineTrongKho = (int)result;

                        if (soLuongVaccineTrongKho < soLuongVaccineTrongCombo)
                        {
                            MessageBox.Show($"Không đủ số lượng vaccine '{maVaccine}' trong kho để bán combo.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra số lượng vaccine và combo trong kho: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
