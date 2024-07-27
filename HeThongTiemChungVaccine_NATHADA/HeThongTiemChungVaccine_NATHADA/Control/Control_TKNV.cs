using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Control_TKNV
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

        //Check trung ma
        public int checkTrungMa(string ma, string table)
        {
            bool trungMa = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("ma_nhanvien") == ma);
            if (trungMa)
            {
                return 1;
            }
            return 0;
        }
        //Check trung ten dang nhap
        public int checkTrungTenDN(string tendn, string table)
        {
            bool trungTenDN = ds.Tables[table].AsEnumerable().Any(row => row.Field<string>("tendangnhap") == tendn);
            if (trungTenDN)
            {
                return 1;
            }
            return 0;
        }

        //Tao tai khoan cho nhan vien moi
        public void insert(Model_TKNV tknv, string table)
        {
            DataRow dr = ds.Tables[table].NewRow();
            dr[0] = tknv.manavi;
            dr[1] = tknv.tdn;
            dr[2] = tknv.matk;
            ds.Tables[table].Rows.Add(dr);
            cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        // Xoa tai khoan nhan vien
        public void delete(Model_TKNV tknv, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(tknv.manavi);
            if (dr != null)
            {
                dr.Delete();
            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);
        }

        public void update(Model_TKNV tknv, string table)
        {
            DataRow dr = ds.Tables[table].Rows.Find(tknv.manavi);
            if (dr != null)
            {
                dr[1] = tknv.tdn;
                dr[2] = tknv.matk;

            }
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            da.Update(ds, table);

        }
    }
}
