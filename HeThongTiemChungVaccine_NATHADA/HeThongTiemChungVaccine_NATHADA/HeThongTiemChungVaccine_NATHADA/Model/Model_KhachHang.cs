using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA.Model
{
    public class Model_KhachHang
    {
        private string maKhachHang;
        public string MaKhachHang
        {
            get { return maKhachHang; }
            set { maKhachHang = value; }
        }

        private string hoTenKhachHang;
        public string HoTenKhachHang
        {
            get { return hoTenKhachHang; }
            set { hoTenKhachHang = value; }
        }

        private string sdtKhachHang;
        public string SdtKhachHang
        {
            get { return sdtKhachHang; }
            set { sdtKhachHang = value; }
        }

        private string emailKhachHang;
        public string EmailKhachHang
        {
            get { return emailKhachHang; }
            set { emailKhachHang = value; }
        }

        private DateTime? ngaySinhKhachHang;
        public DateTime? NgaySinhKhachHang
        {
            get { return ngaySinhKhachHang; }
            set { ngaySinhKhachHang = value; }
        }

        private string gioiTinhKhachHang;
        public string GioiTinhKhachHang
        {
            get { return gioiTinhKhachHang; }
            set { gioiTinhKhachHang = value; }
        }

        private string passKhachHang;
        public string PassKhachHang
        {
            get { return passKhachHang; }
            set { passKhachHang = value; }
        }

        private float diemThanThiet;
        public float DiemThanThiet
        {
            get { return diemThanThiet; }
            set { diemThanThiet = value; }
        }

        private byte? trangThai;
        public byte? TrangThai
        {
            get { return trangThai; }
            set { trangThai = value; }
        }

        // Constructor không tham số
        public Model_KhachHang() { }

        // Constructor với tham số
        public Model_KhachHang(string ma, string hoTen, string sdt, string email, DateTime? ngaySinh, string gioiTinh, string pass, float diem, byte? trangThai)
        {
            this.maKhachHang = ma;
            this.hoTenKhachHang = hoTen;
            this.sdtKhachHang = sdt;
            this.emailKhachHang = email;
            this.ngaySinhKhachHang = ngaySinh;
            this.gioiTinhKhachHang = gioiTinh;
            this.passKhachHang = pass;
            this.diemThanThiet = diem;
            this.trangThai = trangThai;
        }
    }

}
