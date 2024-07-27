using System;
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

    public class Control_Khachhang
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;


        public DataTable select(string table)
        {
            ds = new DataSet();
            string stringSelectKhachhang = "SELECT * FROM " + table;
            SqlCommand cmd = new SqlCommand(stringSelectKhachhang, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }


        public DataTable searchByPhoneNumber(string table, string phoneNumber)
        {
            ds = new DataSet();
            string stringSearch = "SELECT * FROM " + table + " WHERE sdt_khachhang LIKE '%" + phoneNumber + "%'";
            SqlCommand cmd = new SqlCommand(stringSearch, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }

        public void Update(Model_KhachHang khachHang, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(khachHang.MaKhachHang);
            if (dr != null)
            {
                dr[1] = khachHang.HoTenKhachHang;
                dr[2] = khachHang.SdtKhachHang;
                dr[3] = khachHang.EmailKhachHang;
                dr[4] = khachHang.NgaySinhKhachHang;
                dr[5] = khachHang.GioiTinhKhachHang;
                dr[6] = khachHang.PassKhachHang;
                dr[7] = khachHang.DiemThanThiet;
                dr[8] = khachHang.TrangThai;
            }
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }
    }
}
