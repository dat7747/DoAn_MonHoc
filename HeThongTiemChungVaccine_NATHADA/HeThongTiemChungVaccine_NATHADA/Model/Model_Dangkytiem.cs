using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA.Model
{
    class Model_Dangkytiem
    {
        private string maDangky;
        private string hotenDangky;
        private DateTime ngaysinhDangky;
        private string gioiTinh;
        private string diaChi;
        private string hotenNguoilienhe;
        private string moiQuanhe;
        private int sdtnguoiLienhe;
        private string maLoaivaccine;
        private string loaiVaccine;
        private string maVaccine;
        private string maKhachhang;
        private DateTime ngayDangky;
        private DateTime ngayMuontiem;
        private string maCombo;

        public string MaDangky { get => maDangky; set => maDangky = value; }
        public string HotenDangky { get => hotenDangky; set => hotenDangky = value; }
        public DateTime NgaysinhDangky { get => ngaysinhDangky; set => ngaysinhDangky = value; }
        public string GioiTinh { get => gioiTinh; set => gioiTinh = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string HotenNguoilienhe { get => hotenNguoilienhe; set => hotenNguoilienhe = value; }
        public string MoiQuanhe { get => moiQuanhe; set => moiQuanhe = value; }
        public int SdtnguoiLienhe { get => sdtnguoiLienhe; set => sdtnguoiLienhe = value; }
        public string MaLoaivaccine { get => maLoaivaccine; set => maLoaivaccine = value; }
        public string LoaiVaccine { get => loaiVaccine; set => loaiVaccine = value; }
        public string MaVaccine { get => maVaccine; set => maVaccine = value; }
        public string MaKhachhang { get => maKhachhang; set => maKhachhang = value; }
        public DateTime NgayDangky { get => ngayDangky; set => ngayDangky = value; }
        public DateTime NgayMuontiem { get => ngayMuontiem; set => ngayMuontiem = value; }
        public string MaCombo { get => maCombo; set => maCombo = value; }
        
        // Constructor không tham số
        public Model_Dangkytiem() { }
        public Model_Dangkytiem(string maDangky, string hotenDangky, DateTime ngaysinhDangky, string gioiTinh, string diaChi, string hotenNguoilienhe, string moiQuanhe, int sdtnguoiLienhe, string maLoaivaccine, string loaiVaccine, string maVaccine, string maKhachhang, DateTime ngayDangky, DateTime ngayMuontiem, string maCombo) 
        {
            this.maDangky = maDangky;
            this.hotenDangky = hotenDangky;
            this.ngaysinhDangky = ngaysinhDangky;
            this.gioiTinh = gioiTinh;
            this.diaChi = diaChi;
            this.hotenNguoilienhe = hotenNguoilienhe;
            this.moiQuanhe = moiQuanhe;
            this.sdtnguoiLienhe = sdtnguoiLienhe;
            this.maLoaivaccine = maLoaivaccine;
            this.loaiVaccine = loaiVaccine;
            this.maVaccine = maVaccine;
            this.maKhachhang = maKhachhang;
            this.ngayDangky = ngayDangky;
            this.ngayMuontiem = ngayMuontiem;
            this.maCombo = maCombo;

        }
    }

}
