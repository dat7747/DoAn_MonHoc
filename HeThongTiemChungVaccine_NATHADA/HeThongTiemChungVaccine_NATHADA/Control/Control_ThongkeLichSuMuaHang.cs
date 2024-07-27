using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_ThongkeLichSuMuaHang
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        public DataTable LayDuLieuALL()
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
    H.ma_hoadon,
	KH.ma_khachhang,
    CT.thoigian_thanhtoan, 
    KH.hoten_khachhang,
    SUM(thanhtien) AS 'Tổng thành tiền'
FROM HOADON H
JOIN KHACHHANG KH ON H.ma_khachhang = KH.ma_khachhang
JOIN CHITIET_HOADON CT ON H.ma_hoadon = CT.ma_hoadon
GROUP BY H.ma_hoadon,KH.ma_khachhang, CT.thoigian_thanhtoan, KH.hoten_khachhang
ORDER BY CT.thoigian_thanhtoan DESC;

            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dtHoaDon.Load(reader);
            }
            return dtHoaDon;
        }

        public DataTable LayDuLieu(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
    H.ma_hoadon,
	KH.ma_khachhang,
    CT.thoigian_thanhtoan, 
    KH.hoten_khachhang,
    SUM(thanhtien) AS 'Tổng thành tiền'
FROM HOADON H
JOIN KHACHHANG KH ON H.ma_khachhang = KH.ma_khachhang
JOIN CHITIET_HOADON CT ON H.ma_hoadon = CT.ma_hoadon
where CT.thoigian_thanhtoan >= @NgayBatDau AND CT.thoigian_thanhtoan <= @NgayKetThuc
GROUP BY H.ma_hoadon,KH.ma_khachhang, CT.thoigian_thanhtoan, KH.hoten_khachhang
ORDER BY CT.thoigian_thanhtoan DESC;

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


        public DataRow LayKhachHangCoTongThanhTienCaoNhat(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"
            SELECT TOP 1
                KH.ma_khachhang,
                KH.hoten_khachhang,
                SUM(CT.thanhtien) AS 'Tổng thành tiền'
            FROM HOADON H
            JOIN KHACHHANG KH ON H.ma_khachhang = KH.ma_khachhang
            JOIN CHITIET_HOADON CT ON H.ma_hoadon = CT.ma_hoadon
            WHERE CT.thoigian_thanhtoan BETWEEN @NgayBatDau AND @NgayKetThuc
            GROUP BY KH.ma_khachhang, KH.hoten_khachhang
            ORDER BY SUM(CT.thanhtien) DESC;
        ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
