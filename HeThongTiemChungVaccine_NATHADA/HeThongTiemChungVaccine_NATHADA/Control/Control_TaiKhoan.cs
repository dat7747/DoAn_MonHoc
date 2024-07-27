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
    public class Control_TaiKhoan
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public DataTable LayThongTinNguoiDung(string tenDangNhap)
        {
            DataTable dt = new DataTable();

            try
            {
                connect.KetNoiCSDL().Open();
                string query = @"SELECT nv.hoten_nhanvien, nv.diachi_nhanvien, nv.sdt_nhanvien, 
                                    nv.email_nhanvien, nv.cccd_nhanvien, nv.ngaysinh_nhanvien, nv.gioitinh_nhanvien, tk.tendangnhap
                             FROM NHANVIEN nv
                             INNER JOIN TAIKHOAN tk ON nv.ma_nhanvien = tk.ma_nhanvien
                             WHERE tk.tendangnhap = @TenDangNhap";

                SqlCommand cmd = new SqlCommand(query, connect.KetNoiCSDL());
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin người dùng: " + ex.Message);
            }
            finally
            {
                connect.KetNoiCSDL().Close();
            }

            return dt;
        }
    }
}
