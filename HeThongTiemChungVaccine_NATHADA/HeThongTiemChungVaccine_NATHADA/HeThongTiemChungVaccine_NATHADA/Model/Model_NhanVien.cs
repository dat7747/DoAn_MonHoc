using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA
{
    class Model_NhanVien
    {
        private string MaNV;
        public string maNV
        {
            get { return MaNV; }
            set { MaNV = value; }
        }
        private string TenNV;
        public string tenNV
        {
            get { return TenNV; }
            set { TenNV = value; }
        }
        private string DiaChi;
        public string dchi
        {
            get { return DiaChi; }
            set { DiaChi = value; }
        }
        private string SoDienThoai;
        public string sdt
        {
            get { return SoDienThoai; }
            set { SoDienThoai = value; }
        }
        private string Email;
        public string eemail
        {
            get { return Email; }
            set { Email = value; }
        }
        private string CanCuocCD;
        public string cccd
        {
            get { return CanCuocCD; }
            set { CanCuocCD = value; }
        }
        private string NgaySinh;
        public string birthday
        {
            get { return NgaySinh; }
            set { NgaySinh = value; }
        }
        private string GioiTinh;
        public string gtinh
        {
            get { return GioiTinh; }
            set { GioiTinh = value; }
        }
        private string AnhNhanVien;
        public string anhnv
        {
            get { return AnhNhanVien; }
            set { AnhNhanVien = value; }
        }
        public Model_NhanVien(string ma, string ten, string dc, string sdt, string em, string cacu, string ngs,string gti,string anv)
        {

            MaNV = ma;
            TenNV = ten;
            DiaChi = dc;
            SoDienThoai = sdt;
            Email = em;
            CanCuocCD = cacu;
            NgaySinh = ngs;
            GioiTinh = gti;
            AnhNhanVien = anv;
        }
        public Model_NhanVien()
        {

        }
    }
}
