using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_TrangthaitiemVaccine
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        public DataTable select(string table)
        {
            ds = new DataSet();
            string stringSelectDangkytiem = "SELECT tm.STT, tm.ma_dangky AS 'Mã đăng ký', " +
                                   "nd.hoten_nguoitiem AS 'Tên người đăng ký', " +
                                    "vc.ten_vaccine AS 'Tên vaccine', " +
                                    "cb.ten_combo AS 'Thuộc Combo', " +
                                    "tm.mui_vaccine AS 'Mũi vaccine', " +
                                    "CASE " +
                                        "WHEN tm.da_tiem = 0 THEN N'Chưa tiêm' " +
                                        "WHEN tm.da_tiem = 1 THEN N'Đã tiêm' " +
                                    "END AS 'Trạng thái tiêm', tm.ngay_tiem AS 'Ngày tiêm' " +
                                "FROM " + table + " tm " +
                                "LEFT JOIN NGUOITIEM_DANGKY nd ON tm.ma_dangky = nd.ma_dangky " +
                                "LEFT JOIN VACCINE vc ON tm.ma_vaccine = vc.ma_vaccine " +
                                "LEFT JOIN COMBO_VACCINE cb ON tm.ma_combo = cb.ma_combo";
            SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        // Khai báo một phương thức để cập nhật trạng thái tiêm trong cơ sở dữ liệu
        // Khai báo một phương thức để cập nhật trạng thái tiêm trong cơ sở dữ liệu
        public void UpdateStatus(string STT, int newValue, DateTime? ngayTiem)
        {
            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();
                    // Tạo câu lệnh SQL cập nhật trạng thái tiêm và ngày tiêm
                    string query = "UPDATE TIEMVACCINE_MUI SET da_tiem = @newValue, ngay_tiem = @ngayTiem WHERE STT = @STT";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@newValue", newValue);
                        cmd.Parameters.AddWithValue("@STT", STT);
                        if (ngayTiem.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ngayTiem", ngayTiem.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ngayTiem", DBNull.Value);
                        }
                        // Thực thi câu lệnh SQL
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật trạng thái tiêm: " + ex.Message);
            }
        }

        public DataTable SearchData(string table, string keyword)
        {
            ds = new DataSet();
            string stringSelectDangkytiem = "SELECT tm.STT, tm.ma_dangky AS 'Mã đăng ký', " +
                "nd.hoten_nguoitiem AS 'Tên người đăng ký', " +
                "vc.ten_vaccine AS 'Tên vaccine', " +
                "tm.mui_vaccine AS 'Mũi vaccine', " +
                "CASE " +
                    "WHEN tm.da_tiem = 0 THEN N'Chưa tiêm' " +
                    "WHEN tm.da_tiem = 1 THEN N'Đã tiêm' " +
                "END AS 'Trạng thái tiêm' " +
                "FROM " + table + " tm " +
                "LEFT JOIN NGUOITIEM_DANGKY nd ON tm.ma_dangky = nd.ma_dangky " +
                "LEFT JOIN VACCINE vc ON tm.ma_vaccine = vc.ma_vaccine " +
                "WHERE tm.STT LIKE @keyword OR nd.hoten_nguoitiem LIKE @keyword OR vc.ten_vaccine LIKE @keyword OR " +
                "CASE " +
                    "WHEN tm.da_tiem = 0 THEN N'Chưa tiêm' " +
                    "WHEN tm.da_tiem = 1 THEN N'Đã tiêm' " +
                "END LIKE @keyword";

            SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
            cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        // Phương thức lấy ngày tiêm của mũi tiêm cuối cùng
        public DateTime? GetNgayTiemCuoi(string maDangKy)
        {
            DateTime? ngayTiemCuoi = null;

            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    // Tạo câu lệnh SQL để lấy ngày tiêm của mũi tiêm cuối cùng cho maDangKy
                    string query = "SELECT TOP 1 ngay_tiem FROM TIEMVACCINE_MUI WHERE ma_dangky = @maDangKy ORDER BY mui_vaccine DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@maDangKy", maDangKy);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            ngayTiemCuoi = Convert.ToDateTime(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý exception (ném lại hoặc ghi log, tùy vào yêu cầu của bạn)
                throw new Exception("Lỗi khi lấy ngày tiêm của mũi tiêm cuối cùng: " + ex.Message);
            }

            return ngayTiemCuoi;
        }

        public DateTime? GetNgayMuonTiem(string maDangKy)
        {
            DateTime? ngayMuonTiem = null;

            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    // Tạo câu lệnh SQL để lấy ngày_muontiem từ bảng NGUOITIEM_DANGKY
                    string query = "SELECT ngay_muontiem FROM NGUOITIEM_DANGKY WHERE ma_dangky = @maDangKy";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@maDangKy", maDangKy);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            ngayMuonTiem = Convert.ToDateTime(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý exception (ném lại hoặc ghi log, tùy vào yêu cầu của bạn)
                throw new Exception("Lỗi khi lấy ngày muốn tiêm từ NGUOITIEM_DANGKY: " + ex.Message);
            }

            return ngayMuonTiem;
        }
    }
}
