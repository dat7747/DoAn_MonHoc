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
        public static string sevName = @"tcp:dataqlvaccinesql.database.windows.net,1433";
        public static string dbName = "vaccinesql";
        string stringconn = "Data Source =" + sevName + "; Initial Catalog =" + dbName + "; User ID=qlvaccine;Password=Huuthang.1909";
        public SqlConnection KetNoiCSDL()
        {

            SqlConnection conn = new SqlConnection(stringconn);
            return conn;
        }
    }
}
