using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_HoaDon
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public DataTable LayDuLieuHoaDon()
        {
            DataTable dtHoaDon = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT 
                hd.ma_hoadon, 
                kh.hoten_khachhang, 
                vo.ten_voucher, 
                co.ten_combo, 
                va.ten_vaccine, 
                ct.soluong_vaccine, 
                vc.gia_vacine, 
                ct.thanhtien, 
                ct.chietkhau, 
                dk.phi_luukho, 
                ct.thoigian_thanhtoan, 
                ct.hinhthuc_thanhtoan
            FROM HOADON hd
            INNER JOIN CHITIET_HOADON ct ON hd.ma_hoadon = ct.ma_hoadon
            LEFT JOIN VACCINE vc ON ct.ma_vaccine = vc.ma_vaccine
            LEFT JOIN COMBO_VACCINE co ON hd.ma_combo = co.ma_combo
            LEFT JOIN VACCINE va ON ct.ma_vaccine = va.ma_vaccine
            LEFT JOIN NGUOITIEM_DANGKY dk ON hd.ma_dangky = dk.ma_dangky
            LEFT JOIN KHACHHANG kh ON hd.ma_khachhang = kh.ma_khachhang
            LEFT JOIN VOUCHER vo ON hd.ma_voucher = vo.ma_voucher;
            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dtHoaDon.Load(reader);
            }
            return dtHoaDon;
        }
        public DataTable TimKiemHoaDonTheoNgay(DateTime ngayTimKiem)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"
            SELECT 
                hd.ma_hoadon, 
                kh.hoten_khachhang, 
                vo.ten_voucher, 
                co.ten_combo, 
                va.ten_vaccine, 
                ct.soluong_vaccine, 
                vc.gia_vacine, 
                ct.thanhtien, 
                ct.chietkhau, 
                dk.phi_luukho, 
                ct.thoigian_thanhtoan, 
                ct.hinhthuc_thanhtoan
            FROM HOADON hd
            LEFT JOIN CHITIET_HOADON ct ON hd.ma_hoadon = ct.ma_hoadon
            LEFT JOIN KHACHHANG kh ON hd.ma_khachhang = kh.ma_khachhang
            LEFT JOIN VOUCHER vo ON hd.ma_voucher = vo.ma_voucher
            LEFT JOIN COMBO_VACCINE co ON hd.ma_combo = co.ma_combo
            LEFT JOIN VACCINE vc ON ct.ma_vaccine = vc.ma_vaccine
            LEFT JOIN VACCINE va ON ct.ma_vaccine = va.ma_vaccine
            LEFT JOIN NGUOITIEM_DANGKY dk ON hd.ma_dangky = dk.ma_dangky
            WHERE CONVERT(DATE, ct.thoigian_thanhtoan) = @ngayTimKiem";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngayTimKiem", ngayTimKiem);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }


        public DataTable GetHoaDonDataWithStatus1()
        {
            DataTable dataTable = new DataTable();
            try
            {
                string query = @"SELECT hd.ma_hoadon, kh.hoten_khachhang, vo.ten_voucher, co.ten_combo, 
                            va.ten_vaccine, ct.soluong_vaccine, vc.gia_vacine, 
                            ct.thanhtien, ct.chietkhau, ct.philuukho, ct.thoigian_thanhtoan, ct.hinhthuc_thanhtoan
                         FROM HOADON hd
                         INNER JOIN KHACHHANG kh ON hd.ma_khachhang = kh.ma_khachhang
                         INNER JOIN CHITIET_HOADON ct ON hd.ma_hoadon = ct.ma_hoadon
                         INNER JOIN VACCINE vc ON ct.ma_vaccine = vc.ma_vaccine
                         LEFT JOIN VOUCHER vo ON hd.ma_voucher = vo.ma_voucher
                         LEFT JOIN COMBO_VACCINE co ON hd.ma_combo = co.ma_combo
                         LEFT JOIN VACCINE va ON ct.ma_vaccine = va.ma_vaccine
                         WHERE hd.trangthai = 0";

                using (SqlConnection connection =connect.KetNoiCSDL())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataTable;
        }

        public (DataTable, int, double) ThongKeHoaDon(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            // Ví dụ giả định
            string query = @"
        SELECT hd.ma_hoadon, 
               kh.hoten_khachhang, 
               ISNULL(vo.ten_voucher, '') AS ten_voucher, 
               ISNULL(co.ten_combo, '') AS ten_combo, 
               vc.ten_vaccine, 
               ct.soluong_vaccine, 
               vc.gia_vacine, 
               ct.thanhtien, 
               ISNULL(ct.chietkhau, 0) AS chietkhau, 
               ISNULL(ng.phi_luukho, 0) AS phi_luukho, 
               ct.thoigian_thanhtoan, 
               ct.hinhthuc_thanhtoan
        FROM HOADON hd
        INNER JOIN CHITIET_HOADON ct ON hd.ma_hoadon = ct.ma_hoadon
        LEFT JOIN KHACHHANG kh ON hd.ma_khachhang = kh.ma_khachhang
        LEFT JOIN VACCINE vc ON ct.ma_vaccine = vc.ma_vaccine
        LEFT JOIN VOUCHER vo ON hd.ma_voucher = vo.ma_voucher
        LEFT JOIN COMBO_VACCINE co ON hd.ma_combo = co.ma_combo
        LEFT JOIN NGUOITIEM_DANGKY ng ON hd.ma_dangky = ng.ma_dangky
        WHERE ct.thoigian_thanhtoan BETWEEN @ngayBatDau AND @ngayKetThuc";

            SqlCommand command = new SqlCommand(query, connect.KetNoiCSDL());
            command.Parameters.AddWithValue("@ngayBatDau", ngayBatDau);
            command.Parameters.AddWithValue("@ngayKetThuc", ngayKetThuc);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // Tính tổng hóa đơn và tổng thanh toán
            int tongHoaDon = dt.Rows.Count;
            double tongThanhToan = dt.AsEnumerable().Sum(row => row.Field<double>("thanhtien"));

            return (dt, tongHoaDon, tongThanhToan);
        }

    }

}
