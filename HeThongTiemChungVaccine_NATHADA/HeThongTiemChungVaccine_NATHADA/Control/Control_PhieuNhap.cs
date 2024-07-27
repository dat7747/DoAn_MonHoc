using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_PhieuNhap
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        private string LayMaPhieuNhapMoi()
        {
            string maPhieuNhap = "";

            using (SqlConnection conn = connect.KetNoiCSDL())
            {
                conn.Open();

                // Truy vấn cơ sở dữ liệu để lấy mã phiếu nhập cuối cùng
                string query = "SELECT TOP 1 ma_phieunhap FROM PHIEUNHAP ORDER BY ma_phieunhap DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int lastId = int.Parse(result.ToString().Substring(2)); 
                    maPhieuNhap = "PN" + (lastId + 1).ToString("D3"); 
                }
                else
                {
                    maPhieuNhap = "PN001"; 
                }
            }

            return maPhieuNhap;
        }

        public string ThemPhieuNhap(string maNhanVien, string maNhaCungCap, float tongTien)
        {
            string maPhieuNhap = string.Empty;
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    DateTime ngayNhap = DateTime.Now;

                    string queryThemPhieuNhap = "INSERT INTO PHIEUNHAP (ma_phieunhap, ma_nhanvien, ma_nhacungcap, ngay_nhap, tong_tien) VALUES (@maPhieuNhap, @maNhanVien, @maNhaCungCap, @ngayNhap, @tongTien);";
                    SqlCommand cmdThemPhieuNhap = new SqlCommand(queryThemPhieuNhap, connection, transaction);
                    string newMaPhieuNhap = LayMaPhieuNhapMoi();
                    cmdThemPhieuNhap.Parameters.AddWithValue("@maPhieuNhap", newMaPhieuNhap);
                    cmdThemPhieuNhap.Parameters.AddWithValue("@maNhanVien", maNhanVien);
                    cmdThemPhieuNhap.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);
                    cmdThemPhieuNhap.Parameters.AddWithValue("@ngayNhap", ngayNhap);
                    cmdThemPhieuNhap.Parameters.AddWithValue("@tongTien", tongTien);

                    cmdThemPhieuNhap.ExecuteNonQuery();

                    transaction.Commit();
                    maPhieuNhap = newMaPhieuNhap;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Lỗi khi thêm phiếu nhập: " + ex.Message);
                }
            }
            return maPhieuNhap;
        }

        public void ThemChiTietPhieuNhap(string maPhieuNhap, string maVaccine, int soLuong, float giaVaccine)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {

                    string queryThemChiTietPhieuNhap = "INSERT INTO CHITIET_PHIEUNHAP (ma_phieunhap, ma_vaccine, so_luong, gia_vaccine) VALUES (@maPhieuNhap, @maVaccine, @soLuong, @giaVaccine);";
                    SqlCommand cmdThemChiTietPhieuNhap = new SqlCommand(queryThemChiTietPhieuNhap, connection, transaction);
                    cmdThemChiTietPhieuNhap.Parameters.AddWithValue("@maPhieuNhap", maPhieuNhap);
                    cmdThemChiTietPhieuNhap.Parameters.AddWithValue("@maVaccine", maVaccine);
                    cmdThemChiTietPhieuNhap.Parameters.AddWithValue("@soLuong", soLuong);
                    cmdThemChiTietPhieuNhap.Parameters.AddWithValue("@giaVaccine", giaVaccine);

                    cmdThemChiTietPhieuNhap.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Lỗi khi thêm chi tiết phiếu nhập: " + ex.Message);
                }
            }
        }

        private float LayGiaVaccine(string maVaccine)
        {
            float giaVaccine = 0;
            using (SqlConnection conn = connect.KetNoiCSDL())
            {
                conn.Open();
                string query = "SELECT gia_vaccine FROM VACCINE_NCC WHERE ma_vaccine = @maVaccine";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    giaVaccine = Convert.ToSingle(result);
                }
            }
            return giaVaccine;
        }

        public string ThemPhieuNhapVaChiTietPhieuNhap(string maNhanVien, string maNhaCungCap, string maVaccine, int soLuong)
        {
            // Lấy giá vaccine
            float giaVaccine = LayGiaVaccine(maVaccine);

            // Tính tổng tiền
            float tongTien = giaVaccine * soLuong;

            // Thêm phiếu nhập
            string maPhieuNhap = ThemPhieuNhap(maNhanVien, maNhaCungCap, tongTien);

            // Nếu thêm phiếu nhập thành công, thêm chi tiết phiếu nhập
            if (!string.IsNullOrEmpty(maPhieuNhap))
            {
                ThemChiTietPhieuNhap(maPhieuNhap, maVaccine, soLuong, giaVaccine);
            }

            return maPhieuNhap;
        }
        public bool KiemTraTonTaiMaVaccineTrongNhaCungCap(string maVaccine, string maNhaCungCap)
        {
            // Thực hiện truy vấn SQL để kiểm tra xem mã vaccine đã tồn tại trong nhà cung cấp hay chưa
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT COUNT(*) FROM VACCINE_NCC WHERE ma_vaccine = @maVaccine AND ma_nhacungcap = @maNhaCungCap";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);

                connection.Open();
                int count = (int)cmd.ExecuteScalar();
                return count  > 0;
            }
        }
    }
}
