using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Remoting.Contexts;


namespace HeThongTiemChungVaccine_NATHADA.Control
{

    public class Control_Khachhang
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;

        public Control_Khachhang() { }

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
    }
}
