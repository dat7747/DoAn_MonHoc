using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Remoting.Contexts;
using HeThongTiemChungVaccine_NATHADA.Model;
namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_Dangkytiem
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public DataTable SelectAll()
        {
            try
            {
                ds = new DataSet();
                string stringSelectDangkytiem = @"
                            SELECT DISTINCT 
                    nd.ma_dangky,
                    nd.hoten_nguoitiem,
                    nd.ngaysinh_nguoitiem,
                    nd.gioitinh_nguoitiem,
                    nd.diachi_nguoitiem,
                    nd.hoten_nguoilienhe,
                    nd.sdt_nguoilienhe,
                    nd.ma_khachhang, 
                    kh.hoten_khachhang,
                    nd.phi_luukho,
                    nd.tongthanhtoan,
                    nd.ngay_dangky,
                    nd.ngay_muontiem,
                    nd.ma_voucher
                FROM 
                    NGUOITIEM_DANGKY nd
                LEFT JOIN 
                    KHACHHANG kh ON nd.ma_khachhang = kh.ma_khachhang
                LEFT JOIN 
                    NGUOITIEM_MUAVACCINE nmv ON nd.ma_dangky = nmv.ma_dangky
                LEFT JOIN 
                    VACCINE vc ON nmv.ma_vaccine = vc.ma_vaccine
                LEFT JOIN 
                    NGUOITIEM_MUACOMBO nmc ON nd.ma_dangky = nmc.ma_dangky
                LEFT JOIN 
                    COMBO_VACCINE cb ON nmc.ma_combo = cb.ma_combo";

                SqlCommand cmd = new SqlCommand(stringSelectDangkytiem, connect.KetNoiCSDL());
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "DangKyTiem");
                dt = ds.Tables["DangKyTiem"];
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi trong phương thức select: " + ex.Message);
                throw;
            }
        }


        public void InsertHoaDon(string ma_hoadon, string ma_dangky, string ma_khachhang, string ma_voucher, string ma_combo, int trangthai, string manhanvien)
        {
            string query = @"
    INSERT INTO HOADON (ma_hoadon, ma_dangky, ma_khachhang, ma_voucher, ma_combo, trangthai, ma_nhanvien)
    VALUES (@ma_hoadon, @ma_dangky, @ma_khachhang, @ma_voucher, @ma_combo, @trangthai, @ma_nhanvien)";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ma_hoadon", ma_hoadon);
                cmd.Parameters.AddWithValue("@ma_dangky", ma_dangky);
                cmd.Parameters.AddWithValue("@ma_khachhang", ma_khachhang);
                cmd.Parameters.AddWithValue("@ma_voucher", string.IsNullOrEmpty(ma_voucher) ? (object)DBNull.Value : ma_voucher);
                cmd.Parameters.AddWithValue("@ma_combo", string.IsNullOrEmpty(ma_combo) ? (object)DBNull.Value : ma_combo);
                cmd.Parameters.AddWithValue("@trangthai", trangthai);
                cmd.Parameters.AddWithValue("@ma_nhanvien", manhanvien);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertChiTietHoaDon(string ma_hoadon, string maVaccineList, float thanh_tien)
        {
            string query = @"
            INSERT INTO CHITIET_HOADON (ma_hoadon, ma_vaccine, soluong_vaccine, chietkhau, thanhtien, thoigian_thanhtoan, hinhthuc_thanhtoan)
            VALUES (@ma_hoadon, @ma_vaccine, @soluong_vaccine, @chietkhau, @thanhtien, @thoigian_thanhtoan, @hinhthuc_thanhtoan)";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ma_hoadon", ma_hoadon);
                        cmd.Parameters.AddWithValue("@ma_vaccine", maVaccineList);
                        cmd.Parameters.AddWithValue("@soluong_vaccine", DBNull.Value); // Mặc định là null
                        cmd.Parameters.AddWithValue("@chietkhau", DBNull.Value); // Mặc định là null
                        cmd.Parameters.AddWithValue("@thanhtien", thanh_tien);
                        cmd.Parameters.AddWithValue("@thoigian_thanhtoan", DateTime.Now);
                        cmd.Parameters.AddWithValue("@hinhthuc_thanhtoan", "Chuyển khoản");

                        cmd.ExecuteNonQuery();
                    }
                
            }
        }



        public string GetTenVaccine(string ma_vaccine)
        {
            string query = "SELECT ten_vaccine FROM VACCINE WHERE ma_vaccine = @ma_vaccine";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ma_vaccine", ma_vaccine);

                connection.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        public float GetGiaVaccine(string ma_vaccine)
        {
            string query = "SELECT gia_vacine FROM VACCINE WHERE ma_vaccine = @ma_vaccine";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ma_vaccine", ma_vaccine);

                connection.Open();
                return Convert.ToSingle(cmd.ExecuteScalar());
            }
        }

        public List<string> GetMaVaccine(string ma_dangky)
        {
            List<string> maVaccineList = new List<string>();

            string query = @"
            SELECT ma_vaccine 
            FROM NGUOITIEM_MUAVACCINE 
            WHERE ma_dangky = @ma_dangky";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ma_dangky", ma_dangky);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        maVaccineList.Add(reader["ma_vaccine"].ToString());
                    }
                }
            }

            return maVaccineList;
        }

        public List<string> GetMaCombo(string ma_dangky)
        {
            List<string> maComboList = new List<string>();

            string query = @"
            SELECT ma_combo 
            FROM NGUOITIEM_MUACOMBO 
            WHERE ma_dangky = @ma_dangky";

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ma_dangky", ma_dangky);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        maComboList.Add(reader["ma_combo"].ToString());
                    }
                }
            }

            return maComboList;
        }
        public bool KiemTraTonTaiHoaDon(string maDangKy)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT COUNT(*) FROM HOADON WHERE ma_dangky = @maDangKy";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maDangKy", maDangKy);

                connection.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

    }
}

