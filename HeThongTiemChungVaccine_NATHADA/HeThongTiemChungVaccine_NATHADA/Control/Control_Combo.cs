using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Remoting.Contexts;
using HeThongTiemChungVaccine_NATHADA.Model;
namespace HeThongTiemChungVaccine_NATHADA.Control
{
    class Control_Combo
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        public DataTable selectCombo(string table)
        {
            ds = new DataSet();
            string stringSelectDangkytiem = "SELECT * FROM " + table;
            SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        public DataTable selectChiTietCombo(string table, string maCB)
        {
            try
            {
                ds = new DataSet();
                string stringSelectDangkytiem = "SELECT lvc.ten_loaivaccine, vc.ten_vaccine, vc.ma_vaccine, ct.soluong_vaccine"
            + " FROM CHITIET_COMBO_VACCXINE as ct"
            + " JOIN VACCINE as vc ON ct.ma_vaccine = vc.ma_vaccine"
            + " JOIN LOAIVACCINE as lvc ON vc.ma_loaivaccine = lvc.ma_loaivaccine"
            + " WHERE ct.ma_combo = @MaCB";

                SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
                cmd.Parameters.AddWithValue("@MaCB", maCB);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, table);
                dt = ds.Tables[table];
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable select(string table, string maCB)
        {
            try
            {
                ds = new DataSet();
                string stringSelectDangkytiem = "SELECT * FROM " + table + " AS dk, CHITIET_COMBO_VACCXINE AS cb WHERE cb.ma_combo = dk.ma_combo and dk.ma_combo = @MaCB";
                SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
                cmd.Parameters.AddWithValue("@MaCB", maCB);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, table);
                dt = ds.Tables[table];
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // xóa combo
        public bool DeleteComboVaccine(string maCombo)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa combo vaccine này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Tiến hành xóa nếu người dùng đồng ý
                    using (SqlConnection conn = connect.KetNoiCSDL())
                    {
                        conn.Open();
                        string query = "DELETE FROM COMBO_VACCINE WHERE ma_combo = @MaCombo";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaCombo", maCombo);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                }
                else
                {
                    return false; // Trả về false nếu người dùng không đồng ý
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có vẻ dữ liệu này có liên kết khóa ngoại: " + ex.Message);
                return false;
            }
        }

        //thêm combo
        public bool InsertCombo(string tenCombo, float giaCombo)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thêm dữ liệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Kiểm tra người dùng đã chọn Yes hay không
                if (result == DialogResult.Yes)
                {
                    // Thực hiện thêm dữ liệu
                    using (SqlConnection connection = connect.KetNoiCSDL())
                    {
                        connection.Open();

                        string insertQuery = "INSERT INTO COMBO_VACCINE (ten_combo, gia_combo) VALUES (@TenCombo, @GiaCombo)";

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TenCombo", tenCombo);
                            command.Parameters.AddWithValue("@GiaCombo", giaCombo);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Trả về true nếu thêm dữ liệu thành công
                    return true;
                }
                else
                {
                    // Trả về false nếu người dùng không đồng ý
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //sửa combo
        public bool UpdateCombo(string maCombo, string tenCombo, float giaCombo)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật dữ liệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Kiểm tra người dùng đã chọn Yes hay không
                if (result == DialogResult.Yes)
                {
                    // Thực hiện cập nhật dữ liệu
                    using (SqlConnection connection = connect.KetNoiCSDL())
                    {
                        connection.Open();

                        string updateQuery = "UPDATE COMBO_VACCINE SET ten_combo = @TenCombo, gia_combo = @GiaCombo WHERE ma_combo = @MaCombo";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TenCombo", tenCombo);
                            command.Parameters.AddWithValue("@GiaCombo", giaCombo);
                            command.Parameters.AddWithValue("@MaCombo", maCombo);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Trả về true nếu cập nhật dữ liệu thành công
                    return true;
                }
                else
                {
                    // Trả về false nếu người dùng không đồng ý
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //chi tiết combo=====================================================================
        // xóa combo
        public bool DeleteChiTietCombo(string maCombo, string ma_vaccine)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Tiến hành xóa nếu người dùng đồng ý
                    using (SqlConnection conn = connect.KetNoiCSDL())
                    {
                        conn.Open();
                        string query = "DELETE FROM CHITIET_COMBO_VACCXINE WHERE ma_combo = @MaCombo and ma_vaccine = @ma_vaccine";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaCombo", maCombo);
                            cmd.Parameters.AddWithValue("@ma_vaccine", ma_vaccine);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                }
                else
                {
                    return false; // Trả về false nếu người dùng không đồng ý
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có vẻ dữ liệu này có liên kết khóa ngoại: " + ex.Message);
                return false;
            }
        }



        //thêm combo
        public bool InsertChiTietCombo(string maCombo, string maVaccine, int soLuong)
        {
            Console.WriteLine("Số lượng: " + soLuong); // Kiểm tra giá trị số lượng ở đây

            try
            {
                // Hiển thị hộp thoại xác nhận trước khi thêm
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật dòng này?", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = connect.KetNoiCSDL())
                    {
                        conn.Open();
                        string query = "INSERT INTO CHITIET_COMBO_VACCXINE (ma_combo, ma_vaccine, soluong_vaccine) VALUES (@MaCombo, @MaVaccine, @SoLuong)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaCombo", maCombo);
                            cmd.Parameters.AddWithValue("@MaVaccine", maVaccine);
                            cmd.Parameters.AddWithValue("@SoLuong", soLuong);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Người dùng đã hủy Thêm.");
                    return false; // Trả về false nếu người dùng không đồng ý
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //sửa chi tieết combo
        public bool UpdateChiTietCombo(string maCombo, string maVaccine, int soLuong)
        {
            Console.WriteLine($"Thông tin cập nhật chi tiết combo: maCombo={maCombo}, maVaccine={maVaccine}, soLuong={soLuong}");

            try
            {
                // Hiển thị hộp thoại xác nhận trước khi cập nhật
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật dòng này?", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Tiến hành cập nhật nếu người dùng đồng ý
                    using (SqlConnection conn = connect.KetNoiCSDL())
                    {
                        conn.Open();
                        string query = "UPDATE CHITIET_COMBO_VACCXINE SET soluong_vaccine = @SoLuong WHERE ma_combo = @MaCombo AND ma_vaccine = @MaVaccine";
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@MaCombo", maCombo);
                            command.Parameters.AddWithValue("@MaVaccine", maVaccine);
                            command.Parameters.AddWithValue("@SoLuong", soLuong);

                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine($"Rows affected: {rowsAffected}");

                            return rowsAffected > 0;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Người dùng đã hủy cập nhật.");
                    return false; // Trả về false nếu người dùng không đồng ý
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật dữ liệu: " + ex.Message);
                return false;
            }
        }

        public DataTable selectVaccineWithLoaiVaccine()
        {
            ds = new DataSet();
            string query = "SELECT VACCINE.ma_vaccine, VACCINE.ten_vaccine, LOAIVACCINE.ten_loaivaccine " +
                           "FROM VACCINE " +
                           "JOIN LOAIVACCINE ON VACCINE.ma_loaivaccine = LOAIVACCINE.ma_loaivaccine";
            SqlCommand cmd = new SqlCommand(query, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "VACCINE_LOAIVACCINE");
            dt = ds.Tables["VACCINE_LOAIVACCINE"];
            return dt;
        }

    }

}
