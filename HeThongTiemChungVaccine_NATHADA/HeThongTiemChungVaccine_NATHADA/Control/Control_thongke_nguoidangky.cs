using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_thongke_nguoidangky
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        public DataTable LayDuLieuDangKyALL()
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT
                    pdk.ma_dangky,
                    pdk.hoten_nguoilienhe ,
                    pdk.hoten_nguoitiem,
                    pdk.ngay_dangky,
                    pdk.ngay_muontiem
                FROM
                    NGUOITIEM_DANGKY pdk
                LEFT JOIN
                    HOADON hd ON pdk.ma_dangky = hd.ma_dangky
                LEFT JOIN
                    CHITIET_HOADON cthd ON hd.ma_hoadon = cthd.ma_hoadon
                WHERE
                    hd.trangthai = 1 -- Chỉ lấy các hóa đơn đã thanh toán (trangthai = 1)
                GROUP BY
                    pdk.ma_dangky,
                    pdk.hoten_nguoilienhe ,
                    pdk.hoten_nguoitiem,
                    pdk.ngay_dangky,
                    pdk.ngay_muontiem
                ORDER BY
                    pdk.ngay_dangky DESC;

            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dtHoaDon.Load(reader);
            }
            return dtHoaDon;
        }
        public DataTable LayDuLieuDangKy(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT
                    pdk.ma_dangky,
                    pdk.hoten_nguoilienhe ,
                    pdk.hoten_nguoitiem,
                    pdk.ngay_dangky,
                    pdk.ngay_muontiem
                FROM
                    NGUOITIEM_DANGKY pdk
                LEFT JOIN
                    HOADON hd ON pdk.ma_dangky = hd.ma_dangky
                LEFT JOIN
                    CHITIET_HOADON cthd ON hd.ma_hoadon = cthd.ma_hoadon
                WHERE
                    hd.trangthai = 1 -- Chỉ lấy các hóa đơn đã thanh toán (trangthai = 1) 
                    and pdk.ngay_dangky >= @NgayBatDau AND pdk.ngay_dangky <= @NgayKetThuc
                GROUP BY
                    pdk.ma_dangky,
                    pdk.hoten_nguoilienhe ,
                    pdk.hoten_nguoitiem,
                    pdk.ngay_dangky,
                    pdk.ngay_muontiem
                ORDER BY
                    pdk.ngay_dangky DESC;
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dtHoaDon.Load(reader);
            }
            return dtHoaDon;
        }
        // Phương thức để đếm số lượng người đăng ký trong khoảng thời gian cụ thể
        public int DemSoLuongDangKy(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            int soLuong = 0;
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT COUNT(*) FROM NGUOITIEM_DANGKY 
                                 WHERE ngay_dangky >= @NgayBatDau AND ngay_dangky <= @NgayKetThuc;";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                try
                {
                    connection.Open();
                    soLuong = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return soLuong;
        }
    }
}
