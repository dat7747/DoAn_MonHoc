using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Model_Voucher
    {
        private string MaVoucher;
        public string mavoc
        {
            get { return MaVoucher; }
            set { MaVoucher = value; }
        }
        private string TenVoucher;
        public string tenvoc
        {
            get { return TenVoucher; }
            set { TenVoucher = value; }
        }
        private DateTime NgayBD;
        public DateTime ngaybatd
        {
            get { return NgayBD; }
            set { NgayBD = value; }
        }
        private DateTime NgayKT;
        public DateTime ngaykett
        {
            get { return NgayKT; }
            set { NgayKT = value; }
        }
        private float GiamgiaVC;
        public float ggvc
        {
            get { return GiamgiaVC; }
            set { GiamgiaVC = value; }
        }
        public Model_Voucher(string ma, string ten, DateTime datebd, DateTime datekt, float gg)
        {
            MaVoucher = ma;
            TenVoucher = ten;
            NgayBD = datebd;
            NgayKT = datekt;
            GiamgiaVC = gg;
        }
        public Model_Voucher()
        {

        }
    }
}
