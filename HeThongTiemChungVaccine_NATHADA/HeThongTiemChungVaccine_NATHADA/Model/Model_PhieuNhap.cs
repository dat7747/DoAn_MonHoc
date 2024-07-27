using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Model_PhieuNhap
    {
        private string maPhieuNhap;
        public string MaPhieuNhap
        {
            get { return maPhieuNhap; }
            set { maPhieuNhap = value; }
        }

        private string maNhanVien;
        public string MaNhanVien
        {
            get { return maNhanVien; }
            set { maNhanVien = value; }
        }

        private string maNhaCungCap;
        public string MaNhaCungCap
        {
            get { return maNhaCungCap; }
            set { maNhaCungCap = value; }
        }

        private DateTime ngayNhap;
        public DateTime NgayNhap
        {
            get { return ngayNhap; }
            set { ngayNhap = value; }
        }

        private float tongTien;
        public float TongTien
        {
            get { return tongTien; }
            set { tongTien = value; }
        }

        public Model_PhieuNhap(string ma, string nhanvien, string nhacungcap, DateTime ngay, float tongtien)
        {
            maPhieuNhap = ma;
            maNhanVien = nhanvien;
            maNhaCungCap = nhacungcap;
            ngayNhap = ngay;
            tongTien = tongtien;
        }

        public Model_PhieuNhap()
        {

        }
    }

}
