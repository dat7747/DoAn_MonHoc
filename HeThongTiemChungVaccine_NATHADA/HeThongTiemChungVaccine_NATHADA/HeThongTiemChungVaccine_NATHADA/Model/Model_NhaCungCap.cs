using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Model_NhaCungCap
    {
        private string MaNhaCC;
        public string mancc
        {
            get { return MaNhaCC; }
            set { MaNhaCC = value; }
        }
        private string TenNhaCC;
        public string tenncc
        {
            get { return TenNhaCC; }
            set { TenNhaCC = value; }
        }
        private string DiaChiNhaCC;
        public string dcncc
        {
            get { return DiaChiNhaCC; }
            set { DiaChiNhaCC = value; }
        }
        private string SoDTNCC;
        public string sdtncc
        {
            get { return SoDTNCC; }
            set { SoDTNCC = value; }
        }

        public Model_NhaCungCap(string ma, string ten, string diachi, string sdt)
        {
            MaNhaCC = ma;
            TenNhaCC = ten;
            DiaChiNhaCC = diachi;
            SoDTNCC = sdt;
        }
        public Model_NhaCungCap()
        {

        }
    }
}
