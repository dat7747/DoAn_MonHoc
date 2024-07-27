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
    public class Control_Vaccine
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public DataTable Select()
        {
            ds = new DataSet();
            string query = @"
        SELECT 
            V.ma_vaccine, 
            LV.ten_loaivaccine, 
            V.ten_vaccine, 
            V.anh_vaccine, 
            V.thongtin_vaccine, 
            V.doituong, 
            V.phacdolichtiem, 
            V.ngay_san_xuat,
            V.tinhtrangvaccine, 
            V.gia_vacine, 
            V.hansudung_vaccine,  
            V.note, 
            V.phongbenh, 
            V.nguongoc 
        FROM 
            VACCINE V
        JOIN 
            LOAIVACCINE LV ON V.ma_loaivaccine = LV.ma_loaivaccine";

            SqlCommand cmd = new SqlCommand(query, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "VACCINE_LOAIVACCINE");
            return ds.Tables["VACCINE_LOAIVACCINE"];
        }


        private string GenerateNextVaccineCode()
        {
            string lastVaccineCode = GetLastVaccineCode();

            
            if (string.IsNullOrEmpty(lastVaccineCode))
            {
              
                return "VC001";
            }

         
            int lastNumber;
            if (int.TryParse(lastVaccineCode.Substring(2), out lastNumber))
            {
                lastNumber++; 
                return "VC" + lastNumber.ToString("D3");
            }
            else
            {
              
                throw new InvalidOperationException("Không thấy mã Vaccine cuối cùng.");
            }
        }

        private string GetLastVaccineCode()
        {
            string lastVaccineCode = null;

            try
            {
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();
                string query = "SELECT TOP 1 ma_vaccine FROM VACCINE ORDER BY ma_vaccine DESC";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lastVaccineCode = reader["ma_vaccine"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return lastVaccineCode;
        }

        public void AddNewVaccine(string tenVaccine, string maLoaiVaccine, string anhVaccine, string thongTinVaccine, string doiTuong, string phacDo, string tinhTrang, float giaVaccine, DateTime ngaySanXuat, DateTime hanSuDung, string note, string phongBenh, string nguonGoc)
        {
            try
            {
                string newVaccineCode = GenerateNextVaccineCode();

                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    string query = @"INSERT INTO VACCINE (ma_vaccine, ma_loaivaccine, ten_vaccine, anh_vaccine, thongtin_vaccine, doituong, phacdolichtiem, tinhtrangvaccine, gia_vacine, ngay_san_xuat, hansudung_vaccine, note, phongbenh, nguongoc) 
                             VALUES (@MaVaccine, @MaLoaiVaccine, @TenVaccine, @AnhVaccine, @ThongTinVaccine, @DoiTuong, @PhacDoLichTiem, @TinhTrangVaccine, @GiaVaccine, @NgaySanXuat, @HanSuDung, @Note, @PhongBenh, @NguonGoc)";

        
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaVaccine", newVaccineCode);
                        command.Parameters.AddWithValue("@MaLoaiVaccine", maLoaiVaccine);
                        command.Parameters.AddWithValue("@TenVaccine", tenVaccine);
                        command.Parameters.AddWithValue("@AnhVaccine", anhVaccine);
                        command.Parameters.AddWithValue("@ThongTinVaccine", thongTinVaccine);
                        command.Parameters.AddWithValue("@DoiTuong", doiTuong);
                        command.Parameters.AddWithValue("@PhacDoLichTiem", phacDo);
                        command.Parameters.AddWithValue("@TinhTrangVaccine", tinhTrang);
                        command.Parameters.AddWithValue("@GiaVaccine", giaVaccine);
                        command.Parameters.AddWithValue("@NgaySanXuat", ngaySanXuat);
                        command.Parameters.AddWithValue("@HanSuDung", hanSuDung);
                        command.Parameters.AddWithValue("@Note", note);
                        command.Parameters.AddWithValue("@PhongBenh", phongBenh);
                        command.Parameters.AddWithValue("@NguonGoc", nguonGoc);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Lỗi khi thêm Vaccine mới: " + ex.Message);
            }
        }

        public void XoaVaccine(string maVaccine)
        {
            SqlConnection connection = null;
            try
            {
                // Mở kết nối
                connection = connect.KetNoiCSDL();
                connection.Open();

                // Danh sách các bảng tham chiếu đến bảng Vaccine
                List<string> referencingTables = new List<string>
                {
                    "CHITIET_HOADON",
                    "CHITIET_PHIEUNHAP",
                    "CHITIET_COMBO_VACCXINE",
                    "KHO"
                };

                bool hasReference = false;

                foreach (string table in referencingTables)
                {
                    string queryCheckFK = $"SELECT COUNT(*) FROM {table} WHERE ma_vaccine = @MaVaccine";
                    SqlCommand cmdCheckFK = new SqlCommand(queryCheckFK, connection);
                    cmdCheckFK.Parameters.AddWithValue("@MaVaccine", maVaccine);
                    int count = (int)cmdCheckFK.ExecuteScalar();

                    if (count > 0)
                    {
                        hasReference = true;
                        break;
                    }
                }

                if (hasReference)
                {
                    MessageBox.Show("Không thể xóa Vaccine này vì có bản ghi khác tham chiếu đến nó.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string queryDelete = "DELETE FROM VACCINE WHERE ma_vaccine = @MaVaccine";
                    SqlCommand cmdDelete = new SqlCommand(queryDelete, connection);
                    cmdDelete.Parameters.AddWithValue("@MaVaccine", maVaccine);
                    cmdDelete.ExecuteNonQuery();

                    MessageBox.Show("Xóa Vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xóa Vaccine: " + ex.Message);
                throw;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public void UpdateVaccine(string maVaccine, string tenVaccine, string anhVaccine, string thongTinVaccine, string doiTuong, string phacDo, string tinhTrang, float giaVaccine, DateTime ngaySanXuat, DateTime hanSuDung, string note, string phongBenh, string nguonGoc)
        {
            try
            {

                using (SqlConnection conn = connect.KetNoiCSDL())
                {
                    conn.Open();

                    string query = "UPDATE VACCINE SET ten_vaccine = @tenVaccine, anh_vaccine = @anhVaccine, thongtin_vaccine = @thongTinVaccine, doituong = @doiTuong, phacdolichtiem = @phacDo, tinhtrangvaccine = @tinhTrang, gia_vacine = @giaVaccine, ngay_san_xuat = @ngaySanXuat, hansudung_vaccine = @hanSuDung, note = @note, phongbenh = @phongBenh, nguongoc = @nguonGoc WHERE ma_vaccine = @maVaccine";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                        cmd.Parameters.AddWithValue("@tenVaccine", tenVaccine);
                        cmd.Parameters.AddWithValue("@anhVaccine", anhVaccine);
                        cmd.Parameters.AddWithValue("@thongTinVaccine", thongTinVaccine);
                        cmd.Parameters.AddWithValue("@doiTuong", doiTuong);
                        cmd.Parameters.AddWithValue("@phacDo", phacDo);
                        cmd.Parameters.AddWithValue("@tinhTrang", tinhTrang);
                        cmd.Parameters.AddWithValue("@giaVaccine", giaVaccine);
                        cmd.Parameters.AddWithValue("@ngaySanXuat", ngaySanXuat);
                        cmd.Parameters.AddWithValue("@hanSuDung", hanSuDung);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.Parameters.AddWithValue("@phongBenh", phongBenh);
                        cmd.Parameters.AddWithValue("@nguonGoc", nguonGoc);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật Vaccine: " + ex.Message);
            }
        }

        public DataTable LayDanhSachVaccine()
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                string query = "SELECT ma_vaccine, ten_vaccine FROM VACCINE";
                da = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                da.Fill(dt);

                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách vaccine: " + ex.Message);
                throw;
            }
        }
        public DataTable SearchVaccineByName(string tenVaccine)
        {
            try
            {
                string query = @"
            SELECT 
                V.ma_vaccine, 
                LV.ten_loaivaccine, 
                V.ten_vaccine, 
                V.anh_vaccine, 
                V.thongtin_vaccine, 
                V.doituong, 
                V.phacdolichtiem, 
                V.ngay_san_xuat,
                V.tinhtrangvaccine, 
                V.gia_vacine, 
                V.hansudung_vaccine,  
                V.note, 
                V.phongbenh, 
                V.nguongoc 
            FROM 
                VACCINE V
            JOIN 
                LOAIVACCINE LV ON V.ma_loaivaccine = LV.ma_loaivaccine
            WHERE 
                V.ten_vaccine LIKE @TenVaccine";

                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    // Chuẩn bị SqlCommand
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TenVaccine", "%" + tenVaccine + "%"); // Tìm kiếm tên vaccine chứa từ khóa

                    // Đổ dữ liệu vào DataTable
                    dt = new DataTable();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    return dt;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tìm kiếm vaccine: " + ex.Message);
                throw;
            }
        }

    }
}
