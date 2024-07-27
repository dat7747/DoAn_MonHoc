using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Model_TKNV
    {
        private string MaNV;
        public string manavi
        {
            get { return MaNV; }
            set { MaNV = value; }
        }
        private string TenDN;
        public string tdn
        {
            get { return TenDN; }
            set { TenDN = value; }
        }
        private string Matlhau;
        public string matk
        {
            get { return Matlhau; }
            set { Matlhau = value; }
        }


        public Model_TKNV(string ma, string ten, string pass)
        {
            MaNV = ma;
            TenDN = ten;
            Matlhau = pass;
        }
        public Model_TKNV()
        {

        }
    }
}
