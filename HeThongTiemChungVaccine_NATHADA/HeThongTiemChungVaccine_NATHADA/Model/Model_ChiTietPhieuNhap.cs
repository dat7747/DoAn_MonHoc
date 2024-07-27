using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA.Model
{
    public class Model_ChiTietPhieuNhap
    {
        private string maPhieuNhap;
        public string MaPhieuNhap
        {
            get { return maPhieuNhap; }
            set { maPhieuNhap = value; }
        }

        private string maVaccine;
        public string MaVaccine
        {
            get { return maVaccine; }
            set { maVaccine = value; }
        }

        private int soLuong;
        public int SoLuong
        {
            get { return soLuong; }
            set { soLuong = value; }
        }

        private float giaVaccine;
        public float GiaVaccine
        {
            get { return giaVaccine; }
            set { giaVaccine = value; }
        }

        public Model_ChiTietPhieuNhap(string maPhieu, string maVac, int sl, float gia)
        {
            maPhieuNhap = maPhieu;
            maVaccine = maVac;
            soLuong = sl;
            giaVaccine = gia;
        }

        public Model_ChiTietPhieuNhap()
        {

        }
    }

}
