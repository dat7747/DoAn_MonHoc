using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class CustomerReminder
    {
        public string MaKhachHang { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayTiem { get; set; }
    }
}