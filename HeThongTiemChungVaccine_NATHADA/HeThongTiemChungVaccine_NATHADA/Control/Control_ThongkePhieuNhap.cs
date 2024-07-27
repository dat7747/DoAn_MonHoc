using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_ThongkePhieuNhap
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        public DataTable LayDuLieuPhieuNhapALL()
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
                pn.ma_phieunhap,
                nv.ma_nhanvien,
                nv.hoten_nhanvien,
                nc.ten_nhacungcap,
                v.ten_vaccine,
                ctpn.so_luong,
                vncc.gia_vaccine,
                pn.ngay_nhap
            FROM 
                PHIEUNHAP pn
            JOIN 
                NHANVIEN nv ON pn.ma_nhanvien = nv.ma_nhanvien
            JOIN 
                NHACUNGCAP nc ON pn.ma_nhacungcap = nc.ma_nhacungcap
            JOIN 
                CHITIET_PHIEUNHAP ctpn ON pn.ma_phieunhap = ctpn.ma_phieunhap
            JOIN 
                VACCINE_NCC vncc ON ctpn.ma_vaccine = vncc.ma_vaccine
                                 AND pn.ma_nhacungcap = vncc.ma_nhacungcap
            JOIN 
                VACCINE v ON ctpn.ma_vaccine = v.ma_vaccine
            ORDER BY 
                pn.ngay_nhap DESC;

            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dtHoaDon.Load(reader);
            }
            return dtHoaDon;
        }

        public DataTable LayDuLieuPhieuNhap(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
                    pn.ma_phieunhap,
                    nv.ma_nhanvien,
                    nv.hoten_nhanvien,
                    nc.ten_nhacungcap,
                    v.ten_vaccine,
                    ctpn.so_luong,
                    vncc.gia_vaccine,
                    pn.ngay_nhap
                FROM 
                    PHIEUNHAP pn
                JOIN 
                    NHANVIEN nv ON pn.ma_nhanvien = nv.ma_nhanvien
                JOIN 
                    NHACUNGCAP nc ON pn.ma_nhacungcap = nc.ma_nhacungcap
                JOIN 
                    CHITIET_PHIEUNHAP ctpn ON pn.ma_phieunhap = ctpn.ma_phieunhap
                JOIN 
                    VACCINE_NCC vncc ON ctpn.ma_vaccine = vncc.ma_vaccine
                                     AND pn.ma_nhacungcap = vncc.ma_nhacungcap
                JOIN 
                    VACCINE v ON ctpn.ma_vaccine = v.ma_vaccine
                WHERE 
                    pn.ngay_nhap >= @NgayBatDau AND pn.ngay_nhap <= @NgayKetThuc
                ORDER BY 
                    pn.ngay_nhap DESC;
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


        public DataTable LayDuLieuThongKe(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"
                    WITH SoLuongPhieuNhap AS (
                        SELECT COUNT(*) AS SoLuongPhieuNhap
                        FROM PHIEUNHAP
                        WHERE ngay_nhap BETWEEN @NgayBatDau AND @NgayKetThuc
                    ),
                    SoLuongMoiVaccine AS (
                        SELECT 
                            ctpn.ma_vaccine, 
                            v.ten_vaccine, 
                            SUM(ctpn.so_luong) AS SoLuongNhap,
                            SUM(ctpn.so_luong * vncc.gia_vaccine) AS TongGiaNhap
                        FROM 
                            CHITIET_PHIEUNHAP ctpn
                        JOIN 
                            PHIEUNHAP pn ON ctpn.ma_phieunhap = pn.ma_phieunhap
                        JOIN 
                            VACCINE v ON ctpn.ma_vaccine = v.ma_vaccine
                        JOIN 
                            VACCINE_NCC vncc ON ctpn.ma_vaccine = vncc.ma_vaccine
                                            AND pn.ma_nhacungcap = vncc.ma_nhacungcap
                        WHERE 
                            pn.ngay_nhap BETWEEN @NgayBatDau AND @NgayKetThuc
                        GROUP BY 
                            ctpn.ma_vaccine, v.ten_vaccine
                    ),
                    TongGiaNhap AS (
                        SELECT 
                            SUM(ctpn.so_luong * vncc.gia_vaccine) AS TongGiaNhap
                        FROM 
                            CHITIET_PHIEUNHAP ctpn
                        JOIN 
                            PHIEUNHAP pn ON ctpn.ma_phieunhap = pn.ma_phieunhap
                        JOIN 
                            VACCINE_NCC vncc ON ctpn.ma_vaccine = vncc.ma_vaccine
                                            AND pn.ma_nhacungcap = vncc.ma_nhacungcap
                        WHERE 
                            pn.ngay_nhap BETWEEN @NgayBatDau AND @NgayKetThuc
                    )
                    SELECT 
                        slpn.SoLuongPhieuNhap,
                        ISNULL(SUM(slmv.SoLuongNhap), 0) AS TongSoLuongPhieuNhap,
                        ISNULL(SUM(slmv.TongGiaNhap), 0) AS TongGiaNhap,
                        tgn.TongGiaNhap AS TongGiaNhapToanCuc
                    FROM 
                        SoLuongPhieuNhap slpn
                    CROSS JOIN 
                        TongGiaNhap tgn
                    LEFT JOIN 
                        SoLuongMoiVaccine slmv ON 1 = 1
                    GROUP BY 
                        slpn.SoLuongPhieuNhap, tgn.TongGiaNhap;
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
    }
}
