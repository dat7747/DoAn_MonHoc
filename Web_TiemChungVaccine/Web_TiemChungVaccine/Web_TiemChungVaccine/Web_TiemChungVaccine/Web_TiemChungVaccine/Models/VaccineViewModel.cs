using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class VaccineViewModel
    {
        public string MaVaccine { get; set; }
        public string TenVaccine { get; set; }
        public decimal GiaVaccine { get; set; }
        public string Note { get; set; }
        public string MaCombo { get; set; } // Thêm thuộc tính để lưu trữ mã của combo vaccine
    }



}