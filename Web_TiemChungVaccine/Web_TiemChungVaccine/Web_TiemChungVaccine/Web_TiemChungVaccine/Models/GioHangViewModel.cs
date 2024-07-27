using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class GioHangViewModel
    {
        public int IdGioHang { get; set; }
        public string MaVaccine { get; set; }
        public string TenVaccine { get; set; }
        public string AnhVaccine { get; set; }
        public int SoLuong { get; set; }
        public float GiaVaccine { get; set; }
        public float ThanhTien => SoLuong * GiaVaccine;
    }


}