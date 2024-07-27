using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_NhanVien
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        SqlCommandBuilder cB;

        //Hien thi thong tin Nhan vien
        public DataTable select(string table)
        {
            ds = new DataSet();
            string stringselectNV = "select * from " + table;
            SqlCommand cmd = new SqlCommand(stringselectNV, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        //Kiểm tra trùng số điện thoại
        public int checkTrungSDT(string sdt, string table)
        {
            bool trungSDT = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("sdt_nhanvien") == sdt);
            if (trungSDT)
            {
                return 1;
            }
            return 0;
        }

        //Kiểm tra trùng CCCD
        public int checkTrungCCCD(string cccd, string table)
        {
            bool trungCCCD = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("cccd_nhanvien") == cccd);
            if (trungCCCD)
            {
                return 1;
            }
            return 0;
        }
        //Kiểm tra trùng Email
        public int checkTrungEmail(string em, string table)
        {
            bool trungEM = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("email_nhanvien") == em);
            if (trungEM)
            {
                return 1;
            }
            return 0;
        }

        //Kiểm tra mã
        public int checkTrungMa(string ma, string table)
        {
            bool trungMa = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ma_nhanvien") == ma);
            if (trungMa)
            {
                return 1;
            }
            return 0;
        }

        //Kiểm tra ten
        public int checkTrungTen(string ten, string table)
        {
            bool trungTen = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("tendangnhap") == ten);
            if (trungTen)
            {
                return 1;
            }
            return 0;
        }
        private string GenerateNextVaccineCode()
        {
            string lastVaccineCode = GetLastVaccineCode();

            if (string.IsNullOrEmpty(lastVaccineCode))
            {
                return "NV001";
            }

            int lastNumber;
            if (int.TryParse(lastVaccineCode.Substring(3), out lastNumber))
            {
                lastNumber++;
                return "NV" + lastNumber.ToString("D3");
            }
            else
            {
                throw new InvalidOperationException("Không thể lấy được mã Nhân viên cuối cùng.");
            }
        }

        private string GetLastVaccineCode()
        {
            string lastVaccineCode = null;

            try
            {
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();
                string query = "SELECT TOP 1 ma_nhanvien FROM NHANVIEN ORDER BY ma_nhanvien DESC";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lastVaccineCode = reader["ma_nhanvien"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return lastVaccineCode;
        }
        //Them Nhan vien
        public void insert(Model_NhanVien nv, string table)
        {
            nv.maNV = GenerateNextVaccineCode();

            DataRow dr = ds.Tables[table].NewRow();
            dr[0] = nv.maNV;
            dr[1] = nv.tenNV;
            dr[2] = nv.dchi;
            dr[3] = nv.sdt;
            dr[4] = nv.eemail;
            dr[5] = nv.cccd;
            dr[6] = nv.birthday;
            dr[7] = nv.gtinh;
            dr[8] = nv.anhnv;
            dr[9] = nv.qnv;
            ds.Tables[table].Rows.Add(dr);
            cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        //Chinh sua Nhan Vien
        public void update(Model_NhanVien nv, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(nv.maNV);
            if (dr != null)
            {
                dr[1] = nv.tenNV;
                dr[2] = nv.dchi;
                dr[3] = nv.sdt;
                dr[4] = nv.eemail;
                dr[5] = nv.cccd;
                dr[6] = nv.birthday;
                dr[7] = nv.gtinh;
                dr[8] = nv.anhnv;
                dr[9] = nv.qnv;

            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }


        //Xoa Nhân viên
        public void delete(Model_NhanVien nv, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(nv.maNV);
            if (dr != null)
            {
                dr.Delete();
            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        public string LayMaNhanVienTuTenDangNhap(string tenDangNhap)
        {
            string maNhanVien = "";

            try
            {
                // Tạo và mở kết nối đến cơ sở dữ liệu
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    // Tạo câu truy vấn SQL để lấy mã nhân viên từ tên đăng nhập
                    string query = "SELECT ma_nhanvien FROM TAIKHOAN WHERE tendangnhap = @TenDangNhap";

                    // Tạo SqlCommand và thêm tham số
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

                    // Thực thi truy vấn và lấy kết quả
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        maNhanVien = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Lỗi khi lấy mã nhân viên từ tên đăng nhập: " + ex.Message);
            }

            return maNhanVien;
        }


        // Tim kiem
        public DataTable FindMa(string keyword, string tableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = connect.KetNoiCSDL())
            {
                string query = $"SELECT * FROM NHANVIEN WHERE ma_nhanvien LIKE '%{keyword}%' OR hoten_nhanvien LIKE N'%{keyword}%' OR sdt_nhanvien LIKE N'%{keyword}%' OR email_nhanvien LIKE N'%{keyword}%'";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

    }
}
