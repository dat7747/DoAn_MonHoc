using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA
{
    public class Model_Vaccine
    {
        private string maVaccine;
        public string MaVaccine
        {
            get { return maVaccine; }
            set { maVaccine = value; }
        }

        private string maLoaiVaccine;
        public string MaLoaiVaccine
        {
            get { return maLoaiVaccine; }
            set { maLoaiVaccine = value; }
        }

        private string tenVaccine;
        public string TenVaccine
        {
            get { return tenVaccine; }
            set { tenVaccine = value; }
        }

        private string anhVaccine;
        public string AnhVaccine
        {
            get { return anhVaccine; }
            set { anhVaccine = value; }
        }

        private string thongTinVaccine;
        public string ThongTinVaccine
        {
            get { return thongTinVaccine; }
            set { thongTinVaccine = value; }
        }

        private string doiTuong;
        public string DoiTuong
        {
            get { return doiTuong; }
            set { doiTuong = value; }
        }

        private string phacDoLichTiem;
        public string PhacDoLichTiem
        {
            get { return phacDoLichTiem; }
            set { phacDoLichTiem = value; }
        }

        private string tinhTrangVaccine;
        public string TinhTrangVaccine
        {
            get { return tinhTrangVaccine; }
            set { tinhTrangVaccine = value; }
        }

        private float giaVaccine;
        public float GiaVaccine
        {
            get { return giaVaccine; }
            set { giaVaccine = value; }
        }

        private DateTime ngaySanXuat;
        public DateTime NgaySanXuat
        {
            get { return ngaySanXuat; }
            set { ngaySanXuat = value; }
        }

        private DateTime hanSuDungVaccine;
        public DateTime HanSuDungVaccine
        {
            get { return hanSuDungVaccine; }
            set { hanSuDungVaccine = value; }
        }


        private string note;
        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        private string phongBenh;
        public string PhongBenh
        {
            get { return phongBenh; }
            set { phongBenh = value; }
        }

        private string nguonGoc;
        public string NguonGoc
        {
            get { return nguonGoc; }
            set { nguonGoc = value; }
        }

        // Constructor không tham số
        public Model_Vaccine() { }

        // Constructor với tham số
        public Model_Vaccine(string maVaccine, string maLoaiVaccine, string tenVaccine, string anhVaccine, string thongTinVaccine, string doiTuong, string phacDoLichTiem, string tinhTrangVaccine, float giaVaccine, DateTime hanSuDungVaccine, DateTime ngaySanXuat, string note, string phongBenh, string nguonGoc)
        {
            this.maVaccine = maVaccine;
            this.maLoaiVaccine = maLoaiVaccine;
            this.tenVaccine = tenVaccine;
            this.anhVaccine = anhVaccine;
            this.thongTinVaccine = thongTinVaccine;
            this.doiTuong = doiTuong;
            this.phacDoLichTiem = phacDoLichTiem;
            this.tinhTrangVaccine = tinhTrangVaccine;
            this.giaVaccine = giaVaccine;
            this.ngaySanXuat = ngaySanXuat;
            this.hanSuDungVaccine = hanSuDungVaccine;
            this.note = note;
            this.phongBenh = phongBenh;
            this.nguonGoc = nguonGoc;
        }
    }

}
