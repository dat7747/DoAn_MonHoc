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
using System.Data;
namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class FormThongTinChiTietNguoiTiem : Form
    {
        DataTable dtDetail;

        public FormThongTinChiTietNguoiTiem(DataTable dt)
        {
            InitializeComponent();
            dtDetail = dt;
            // Đặt FormBorderStyle thành FixedDialog để ngăn người dùng chỉnh sửa kích thước form
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void FormThongTinChiTietNguoiTiem_Load(object sender, EventArgs e)
        {
            if (dtDetail != null && dtDetail.Rows.Count > 0)
            {
                DataRow row = dtDetail.Rows[0];

                // Điền dữ liệu vào các TextBox
                textBox1.Text = row["hoten_nguoitiem"].ToString();
                textBox2.Text = row["ngaysinh_nguoitiem"].ToString();
                textBox3.Text = row["gioitinh_nguoitiem"].ToString();
                textBox4.Text = row["diachi_nguoitiem"].ToString();
                dateTimePicker1.Value = DateTime.Parse(row["ngay_muontiem"].ToString());
                textBox6.Text = row["hoten_nguoilienhe"].ToString();
                textBox7.Text = row["moiquanhe_nguoitiem"].ToString();
                textBox8.Text = row["sdt_nguoilienhe"].ToString();
                textBox9.Text = row["ngay_dangky"].ToString();
                textBox10.Text = row["loai_vaccine"].ToString();
                textBox5.Text = row["ten_combo"].ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các TextBox
            string maDangKy = dtDetail.Rows[0]["ma_dangky"].ToString();
            string hoTenNguoiTiem = textBox1.Text;
            string ngaySinh = textBox2.Text;
            string gioiTinh = textBox3.Text;
            string diaChi = textBox4.Text;
            string ngayMuonTiem = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string hoTenNguoiLienHe = textBox6.Text;
            string moiQuanHe = textBox7.Text;
            string sdtNguoiLienHe = textBox8.Text;
            string ngayDangKy = textBox9.Text;
            string loaiVaccine = textBox10.Text;

            // Kiểm tra các điều kiện
            if (string.IsNullOrEmpty(hoTenNguoiTiem) || string.IsNullOrEmpty(ngaySinh) || string.IsNullOrEmpty(gioiTinh) || string.IsNullOrEmpty(diaChi) || string.IsNullOrEmpty(ngayMuonTiem) || string.IsNullOrEmpty(hoTenNguoiLienHe) || string.IsNullOrEmpty(moiQuanHe) || string.IsNullOrEmpty(sdtNguoiLienHe) || string.IsNullOrEmpty(ngayDangKy) || string.IsNullOrEmpty(loaiVaccine))
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (DateTime.Parse(ngayMuonTiem) <= DateTime.Parse(ngayDangKy))
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show("Ngày tiêm không được trước ngày đăng ký.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Cập nhật dữ liệu vào cơ sở dữ liệu
                UpdateData(maDangKy, hoTenNguoiTiem, ngaySinh, gioiTinh, diaChi, ngayMuonTiem, hoTenNguoiLienHe, moiQuanHe, sdtNguoiLienHe, ngayDangKy, loaiVaccine);

                // Hiển thị thông báo thành công
                MessageBox.Show("Dữ liệu đã được cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void UpdateData(string maDangKy, string hoTenNguoiTiem, string ngaySinh, string gioiTinh, string diaChi, string ngayMuonTiem, string hoTenNguoiLienHe, string moiQuanHe, string sdtNguoiLienHe, string ngayDangKy, string loaiVaccine)
        {
            // Kiểm tra không để trống các trường dữ liệu
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection conn = new ConnSQL().KetNoiCSDL())
            {
                conn.Open();
                string query = "UPDATE NGUOITIEM_DANGKY SET hoten_nguoitiem = @hoTenNguoiTiem, ngaysinh_nguoitiem = @ngaySinh, gioitinh_nguoitiem = @gioiTinh, diachi_nguoitiem = @diaChi, hoten_nguoilienhe = @hoTenNguoiLienHe, moiquanhe_nguoitiem = @moiQuanHe, sdt_nguoilienhe = @sdtNguoiLienHe, ngay_dangky = @ngayDangKy, loai_vaccine = @loaiVaccine, ngay_muontiem = @ngayMuonTiem WHERE ma_dangky = @maDangKy";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hoTenNguoiTiem", hoTenNguoiTiem);
                    cmd.Parameters.AddWithValue("@ngaySinh", DateTime.Parse(ngaySinh));
                    cmd.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@diaChi", diaChi);
                    cmd.Parameters.AddWithValue("@ngayMuonTiem", DateTime.Parse(ngayMuonTiem));
                    cmd.Parameters.AddWithValue("@hoTenNguoiLienHe", hoTenNguoiLienHe);
                    cmd.Parameters.AddWithValue("@moiQuanHe", moiQuanHe);
                    cmd.Parameters.AddWithValue("@sdtNguoiLienHe", int.Parse(sdtNguoiLienHe));
                    cmd.Parameters.AddWithValue("@ngayDangKy", DateTime.Parse(ngayDangKy));
                    cmd.Parameters.AddWithValue("@loaiVaccine", loaiVaccine);
                    cmd.Parameters.AddWithValue("@maDangKy", maDangKy);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
