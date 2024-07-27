using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Control_LoaiVC
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        SqlCommandBuilder cB;

        public DataTable select(string table)
        {
            ds = new DataSet();
            string stringselectLVC = "select * from " + table;
            SqlCommand cmd = new SqlCommand(stringselectLVC, connect.KetNoiCSDL());
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, table);
            dt = ds.Tables[table];
            return dt;
        }
        public int checkTrungMa(string ma, string table)
        {
            bool trungMa = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ma_loaivaccine") == ma);
            if (trungMa)
            {
                return 1;
            }
            return 0;
        }
        public int checkTrungTen(string ten, string table)
        {
            bool trungTen = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ten_loaivaccine") == ten);
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
                return "LVC001";
            }

            int lastNumber;
            if (int.TryParse(lastVaccineCode.Substring(3), out lastNumber))
            {
                lastNumber++;
                return "LVC" + lastNumber.ToString("D3");
            }
            else
            {
                throw new InvalidOperationException("Không thể lấy được mã Vaccine cuối cùng.");
            }
        }

        private string GetLastVaccineCode()
        {
            string lastVaccineCode = null;

            try
            {
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();
                string query = "SELECT TOP 1 ma_loaivaccine FROM LOAIVACCINE ORDER BY ma_loaivaccine DESC";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lastVaccineCode = reader["ma_loaivaccine"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return lastVaccineCode;
        }
        public void insert(Model_LoaiVaccine lvc, string table)
        {
            lvc.maLoai = GenerateNextVaccineCode();

            DataRow dr = ds.Tables[table].NewRow();
            dr[0] = lvc.maLoai;
            dr[1] = lvc.tenLoai;
            ds.Tables[table].Rows.Add(dr);
            cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }
        public void update(Model_LoaiVaccine lvc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(lvc.maLoai);
            if (dr != null)
            {
                dr[1] = lvc.tenLoai;

            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }

        public void delete(Model_LoaiVaccine lvc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(lvc.maLoai);
            if (dr != null)
            {
                dr.Delete();
            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        public string GetTenLoaiVaccine(string maLoaiVaccine)
        {
            string tenLoaiVaccine = "";
            try
            {
                // Mở kết nối
                connect.KetNoiCSDL().Open();

                string query = "SELECT ten_loaivaccine FROM LOAIVACCINE WHERE ma_loaivaccine = @maLoaiVaccine";
                SqlCommand cmd = new SqlCommand(query, connect.KetNoiCSDL());
                cmd.Parameters.AddWithValue("@maLoaiVaccine", maLoaiVaccine);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tenLoaiVaccine = reader["ten_loaivaccine"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (nếu cần)
                Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
            }
            finally
            {
                if (connect.KetNoiCSDL().State == ConnectionState.Open)
                {
                    connect.KetNoiCSDL().Close();
                }
            }
            return tenLoaiVaccine;
        }

        public DataTable FindMa(string keyword, string tableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = connect.KetNoiCSDL())
            {
                string query = $"SELECT * FROM LOAIVACCINE WHERE ma_loaivaccine LIKE '%{keyword}%' OR ten_loaivaccine LIKE N'%{keyword}%'";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

    }
}
