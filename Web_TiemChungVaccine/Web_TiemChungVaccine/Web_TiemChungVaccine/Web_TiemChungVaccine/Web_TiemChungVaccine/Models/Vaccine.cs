using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class Vaccine
    {
        QL_VACCINEDataContext db = new QL_VACCINEDataContext();
        public string MaVC { get; set; }
        public string MaLVC { get; set; }
        public string TenVC { get; set; }
        public string AnhVC { get; set; }
        public string ThongTinVC { get; set; }
        public string DoiTuongVC { get; set; }
        public string PhacDo { get; set; }
        public string TinhTrang { get; set; }
        public float GiaVC { get; set; }
        public DateTime DateSX { get; set; }
        public DateTime DateHSD { get; set; }
        public string NoteVC { get; set; }
        public string PhongbenhVC { get; set; }
        public string NguongocVC { get; set; }

        public Vaccine(string ms)
        {
            MaLVC = ms;
            VACCINE vc = db.VACCINEs.FirstOrDefault(i => i.ma_vaccine == ms);
            TenVC = vc.ten_vaccine;
        }
        public Vaccine(string ma_vaccine, string ma_dangky)
        {
            MaVC = ma_vaccine;
            VACCINE vc = db.VACCINEs.FirstOrDefault(i => i.ma_vaccine == ma_vaccine);
            TenVC = vc.ten_vaccine;
            // Other properties
        }
    }
}