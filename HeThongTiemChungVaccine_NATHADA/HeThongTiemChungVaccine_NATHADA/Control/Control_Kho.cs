using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA
{
    public  class Control_Kho
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public DataTable LayDanhSachKho()
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                string query = @"
            SELECT 
                V.ma_vaccine,
                V.ten_vaccine,
                K.soluong_vaccine,
                K.donvitinh
            FROM 
                KHO K
            INNER JOIN 
                VACCINE V ON K.ma_vaccine = V.ma_vaccine";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Lỗi khi lấy danh sách kho: " + ex.Message);
                throw;
            }
        }

        public void ThemDuLieu(string maVaccine, int soLuong, string donViTinh)
        {
            SqlConnection conn = null;
            try
            {
                conn = connect.KetNoiCSDL();
                conn.Open();

                if (KiemTraVaccineTonTaiTrongKho(maVaccine))
                {
                    MessageBox.Show("Vaccine đã tồn tại trong kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "INSERT INTO KHO (ma_vaccine, soluong_vaccine, donvitinh) " +
                               "VALUES (@MaVaccine, @SoLuong, @DonViTinh)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaVaccine", maVaccine);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                cmd.Parameters.AddWithValue("@DonViTinh", donViTinh);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Thêm Vaccine vào kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm Vaccine vào kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private bool KiemTraVaccineTonTaiTrongKho(string maVaccine)
        {
            SqlConnection conn = null;
            try
            {
                conn = connect.KetNoiCSDL();
                conn.Open();

                string query = "SELECT COUNT(*) FROM KHO WHERE ma_vaccine = @MaVaccine";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaVaccine", maVaccine);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra Vaccine trong kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public void XoaDuLieu(string maVaccine)
        {
            try
            {
                if (!KiemTraVaccineTonTaiTrongKho(maVaccine))
                {
                    MessageBox.Show("Vaccine không tồn tại trong kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = connect.KetNoiCSDL())
                {
                    conn.Open();
                    string query = "DELETE FROM KHO WHERE ma_vaccine = @MaVaccine";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaVaccine", maVaccine);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Xóa Vaccine khỏi kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa Vaccine khỏi kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SuaDuLieu(string maVaccine, int soLuong, string donViTinh)
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                if (!KiemTraVaccineTonTaiTrongKho(maVaccine))
                {
                    MessageBox.Show("Vaccine không tồn tại trong kho. Không thể sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "UPDATE KHO SET soluong_vaccine = @SoLuong, donvitinh = @DonViTinh " +
                               "WHERE ma_vaccine = @MaVaccine ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                cmd.Parameters.AddWithValue("@DonViTinh", donViTinh);
                cmd.Parameters.AddWithValue("@MaVaccine", maVaccine);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Sửa thông tin Vaccine trong kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa thông tin Vaccine trong kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Đóng kết nối sau khi thực hiện xong
                connect.KetNoiCSDL().Close();
            }
        }

        public DataTable TimKiemTheoTenVaccine(string keyword)
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                string query = @"
            SELECT 
                V.ma_vaccine ,
                V.ten_vaccine,
                K.soluong_vaccine,
                K.donvitinh 
            FROM 
                KHO K
            INNER JOIN 
                VACCINE V ON K.ma_vaccine = V.ma_vaccine
            WHERE 
                V.ten_vaccine LIKE @Keyword";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm Vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
 