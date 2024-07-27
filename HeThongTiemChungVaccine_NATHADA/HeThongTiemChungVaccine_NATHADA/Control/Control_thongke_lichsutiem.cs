using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_thongke_lichsutiem
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
                string query = @"SELECT NT.ma_dangky, VC.ten_vaccine, TM.mui_vaccine, TM.ngay_tiem, NT.hoten_nguoitiem
FROM TIEMVACCINE_MUI TM
JOIN NGUOITIEM_DANGKY NT ON TM.ma_dangky = NT.ma_dangky
JOIN VACCINE VC ON VC.ma_vaccine =  TM.ma_vaccine
WHERE TM.da_tiem = 1;


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
                string query = @"


                SELECT NT.ma_dangky, VC.ten_vaccine, TM.mui_vaccine, TM.ngay_tiem, NT.hoten_nguoitiem
                FROM TIEMVACCINE_MUI TM
                JOIN NGUOITIEM_DANGKY NT ON TM.ma_dangky = NT.ma_dangky
                JOIN VACCINE VC ON VC.ma_vaccine =  TM.ma_vaccine
                WHERE TM.da_tiem = 1
                AND TM.ngay_tiem BETWEEN @NgayBatDau AND @NgayKetThuc;
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
        public int DemSoLuong(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            int soLuong = 0;
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
                    COUNT(*) AS so_luong_tiem
                FROM TIEMVACCINE_MUI TM
                JOIN NGUOITIEM_DANGKY NT ON TM.ma_dangky = NT.ma_dangky
                WHERE TM.da_tiem = 1
                AND TM.ngay_tiem BETWEEN @NgayBatDau AND @NgayKetThuc";

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
