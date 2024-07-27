using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class VoucherViewModel
    {
        public string MaVoucher { get; set; }
        public string TenVoucher { get; set; }
        public DateTime NgayBD { get; set; }
        public DateTime NgayKT { get; set; }
        public float Giamgia { get; set; } 
    }
}