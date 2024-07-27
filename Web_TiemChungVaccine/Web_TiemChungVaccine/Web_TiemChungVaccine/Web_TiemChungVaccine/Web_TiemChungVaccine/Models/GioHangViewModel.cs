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
        public string MaCombo { get; set; }
        public string TenCombo { get; set; }
        public float GiaCombo { get; set; }

        // Tính thành tiền dựa vào loại mặt hàng
        public float ThanhTien
        {
            get
            {
                if (!string.IsNullOrEmpty(MaCombo))
                {
                    // Nếu là combo, tính tổng tiền của combo
                    return SoLuong * GiaCombo;
                }
                else
                {
                    // Nếu là vaccine đơn lẻ, tính tổng tiền của vaccine đó
                    return SoLuong * GiaVaccine;
                }
            }
        }
    }


}