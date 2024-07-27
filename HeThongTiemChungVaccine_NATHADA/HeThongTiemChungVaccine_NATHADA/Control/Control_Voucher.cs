using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_Voucher
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        SqlCommandBuilder cB;

        //Hien thi thong tin Voucher
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
            bool trungMa = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ma_voucher") == ma);
            if (trungMa)
            {
                return 1;
            }
            return 0;
        }

        public int checkTrungTen(string ten, string table)
        {
            bool trungTen = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ten_voucher") == ten);
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
                return "VCH001";
            }

            int lastNumber;
            if (int.TryParse(lastVaccineCode.Substring(3), out lastNumber))
            {
                lastNumber++;
                return "VCH" + lastNumber.ToString("D3");
            }
            else
            {
                throw new InvalidOperationException("Không thể lấy được mã Voucher cuối cùng.");
            }
        }

        private string GetLastVaccineCode()
        {
            string lastVaccineCode = null;

            try
            {
                SqlConnection connection = connect.KetNoiCSDL();
                connection.Open();
                string query = "SELECT TOP 1 ma_voucher FROM VOUCHER ORDER BY ma_voucher DESC";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lastVaccineCode = reader["ma_voucher"].ToString();
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
        public void insert(Model_Voucher voc, string table)
        {
            voc.mavoc = GenerateNextVaccineCode();

            DataRow dr = ds.Tables[table].NewRow();
            dr[0] = voc.mavoc;
            dr[1] = voc.tenvoc;
            dr[2] = voc.ngaybatd;
            dr[3] = voc.ngaykett;
            dr[4] = voc.ggvc;
            ds.Tables[table].Rows.Add(dr);
            cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        //Chinh sua Voucher
        public void update(Model_Voucher voc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(voc.mavoc);
            if (dr != null)
            {
                dr[1] = voc.tenvoc;
                dr[2] = voc.ngaybatd;
                dr[3] = voc.ngaykett;
                dr[4] = voc.ggvc;

            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }

        //Xoa Voucher
        public void delete(Model_Voucher voc, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(voc.mavoc);
            if (dr != null)
            {
                dr.Delete();
            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }
    }
}
