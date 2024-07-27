using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace HeThongTiemChungVaccine_NATHADA
{
    class ConnSQL
    {
        public static string sevName = @"LAPTOP-IRLS8GIA";
        public static string dbName = "TiemChungVC2205";
        string stringconn = "Data Source =" + sevName + "; Initial Catalog =" + dbName + "; User ID=sa;Password=123";
        public SqlConnection KetNoiCSDL()
        {

            SqlConnection conn = new SqlConnection(stringconn);
            return conn;
        }
    }
}
