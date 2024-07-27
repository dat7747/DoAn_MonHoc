using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_NhaCungCap
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        SqlCommandBuilder cB;

        //Hien thi thong tin Nha Cung Cap
        public DataTable select(string table)
        {
            ds = new DataSet();
            string stringselectNCC = "select * from " + table;
            SqlCommand cmd = new SqlCommand(stringselectNCC, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        public int checkTrungMa(string ma, string table)
        {
            bool trungMa = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ma_nhacungcap") == ma);
            if (trungMa)
            {
                return 1;
            }
            return 0;
        }
        public int checkTrungTen(string ten, string table)
        {
            bool trungTen = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ten_nhacungcap") == ten);
            if (trungTen)
            {
                return 1;
            }
            return 0;
        }
        public int checkTrungSDT(string sdt, string table)
        {
            bool trungTen = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("sdt_nhacungcap") == sdt);
            if (trungTen)
            {
                return 1;
            }
            return 0;
        }
        public DataTable LayDanhSachNhaCungCap()
        {
            try
            {
                SqlConnection conn = connect.KetNoiCSDL();
                conn.Open();

                string query = "SELECT ma_nhacungcap, ten_nhacungcap FROM NHACUNGCAP";
                da = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                da.Fill(dt);

                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách nhà cung cấp: " + ex.Message);
                throw;
            }
        }
        private string GenerateNextVaccineCode()
        {
            string lastVaccineCode = GetLastVaccineCode();

            if (string.IsNullOrEmpty(lastVaccineCode))
            {
                return "NCC001";
            }

            int lastNumber;
            if (int.TryParse(lastVaccineCode.Substring(3), out lastNumber))
            {
                lastNumber++;
                return "NCC" + lastNumber.ToString("D3");
            }
            else
            {
                throw new InvalidOperationException("Không thể lấy được mã Nhà Cung Cấp cuối cùng.");
            }
        }

        private string GetLastVaccineCode()
        {
            string lastVaccineCode = null;

            try
            {
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();
                string query = "SELECT TOP 1 ma_nhacungcap FROM NHACUNGCAP ORDER BY ma_nhacungcap DESC";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lastVaccineCode = reader["ma_nhacungcap"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return lastVaccineCode;
        }
        //Them Nha cung Cap
        public void insert(Model_NhaCungCap ncc, string table)
        {
            //DataRow dr = ds.Tables[table].NewRow();
            //dr[0] = lvc.maLoai;
            //dr[1] = lvc.tenLoai;
            //ds.Tables[table].Rows.Add(dr);
            //cB = new SqlCommandBuilder(da);
            //da.Update(ds, table);
            // Lấy mã loại vaccine tiếp theo
            ncc.mancc = GenerateNextVaccineCode();

            DataRow dr = ds.Tables[table].NewRow();
            dr[0] = ncc.mancc;
            dr[1] = ncc.tenncc;
            dr[2] = ncc.dcncc;
            dr[3] = ncc.sdtncc;
            ds.Tables[table].Rows.Add(dr);
            cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        //Chinh sua Nha Cung Cap
        public void update(Model_NhaCungCap ncc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(ncc.mancc);
            if (dr != null)
            {
                dr[1] = ncc.tenncc;
                dr[2] = ncc.dcncc;
                dr[3] = ncc.sdtncc;

            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }

        //Xoa Nha Cung Cap
        public void delete(Model_NhaCungCap ncc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(ncc.mancc);
            if (dr != null)
            {
                dr.Delete();
            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }


        // Tim kiem
        public DataTable FindMa(string keyword, string tableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = connect.KetNoiCSDL())
            {
                string query = $"SELECT * FROM NHACUNGCAP WHERE ma_nhacungcap LIKE '%{keyword}%' OR ten_nhacungcap LIKE N'%{keyword}%' OR sdt_nhacungcap LIKE N'%{keyword}%'";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }
    }
}
