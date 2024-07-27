using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Model_LoaiVaccine
    {
        private string MaLoaiVC;
        public string maLoai
        {
            get { return MaLoaiVC; }
            set { MaLoaiVC = value; }
        }
        private string TenLoaiVC;
        public string tenLoai
        {
            get { return TenLoaiVC; }
            set { TenLoaiVC = value; }
        }

        public Model_LoaiVaccine(string ma, string ten)
        {
            MaLoaiVC = ma;
            TenLoaiVC = ten;
        }
        public Model_LoaiVaccine()
        {

        }
    }
}
