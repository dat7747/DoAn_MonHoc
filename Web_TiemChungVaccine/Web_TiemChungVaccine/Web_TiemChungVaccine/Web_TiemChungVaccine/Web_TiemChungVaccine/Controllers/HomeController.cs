using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TiemChungVaccine.Models;
namespace Web_TiemChungVaccine.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        QL_VACCINEDataContext db = new QL_VACCINEDataContext();
        // GET: Home
        //public ActionResult Index(/*int page = 1*/)
        //{
        //    List<VACCINE> a = db.VACCINEs.ToList();
        //    return View(a);
        //}
        public ActionResult Index(/*int page = 1*/)
        {
            var vaccinesInStock = db.VACCINEs
                            .Join(db.KHOs,
                                  v => v.ma_vaccine,
                                  k => k.ma_vaccine,
                                  (v, k) => v)
                            .ToList();

            return View(vaccinesInStock);
        }
        //public ActionResult Voucher(/*int page = 1*/)
        //{
        //    List<VOUCHER> a = db.VOUCHERs.ToList();
        //    return View(a);
        //}
        public ActionResult IndexCombo(/*int page = 1*/)
        {
            List<CHITIET_COMBO_VACCXINE> a = db.CHITIET_COMBO_VACCXINEs.ToList();
            return View(a);
        }
        public ActionResult XemChiTietCombo(string id)
        {
            CHITIET_COMBO_VACCXINE s = db.CHITIET_COMBO_VACCXINEs.FirstOrDefault(i => i.ma_combo == id);
            Session["macombo"] = s.ma_combo;
            Session["mavaccine"] = s.ma_vaccine;
            return View(s);
        }

        //Gioi thieu
        public ActionResult Gioithieu()
        {
            return View();
        }

        //Chinh sach bao mat
        public ActionResult Chinhsachbaomat()
        {
            return View();
        }

        //Nhung dieu can biet
        public ActionResult Nhungdieucanbiet()
        {
            return View();
        }

        //Vi sao nguoi lon nen tiem chung
        public ActionResult Visaonguoilonnentiemchung()
        {
            return View();
        }

        //Quy trinh tiem chung
        public ActionResult Quytrinhtiemchung()
        {
            return View();
        }

        //Cam nang tiem chung
        public ActionResult Camnangtiemchung()
        {
            return View();
        }

        //Tra gop 0% lai suat
        public ActionResult Tragop()
        {
            return View();
        }

        //Cau hoi thuong gap
        public ActionResult Cauhoithuonggap()
        {
            return View();
        }

        //Dich vu tiem chung theo yeu cau
        public ActionResult Tiemchungtheoyeucau()
        {
            return View();
        }

        //Lich tiem chung cho nguoi lon
        public ActionResult Lichtiemchungnguoilon()
        {
            return View();
        }

        //Lich tiem chung cho tre em
        public ActionResult Lichtiemchungtreem()
        {
            return View();
        }

        //Bang gia tiem chung
        public ActionResult BangGiaTiemChung()
        {
            List<VACCINE> a = db.VACCINEs.ToList();
            return View(a);
        }
        //Xem chi tiết vaccine
        public ActionResult XemChiTietVaccine(string id) 
        {
            VACCINE s = db.VACCINEs.FirstOrDefault(i => i.ma_vaccine == id);
            Session["mavaccine"] = s.ma_vaccine;
            Session["tenvaccine"] = s.ten_vaccine;
            return View(s);
        }

        //Khách Hàng xem chi tiết tra cu Thong tin dang ky tiem
        public ActionResult XemChiTietThongTinDangKyTiem(string id)
        {
            NGUOITIEM_DANGKY s = db.NGUOITIEM_DANGKies.FirstOrDefault(i => i.ma_dangky == id);
            Session["madangky"] = s.ma_dangky;
            Session["makhachhang"] = s.ma_khachhang;
            return View(s);
        }

        //======================== Tìm kiếm =========================
        public ActionResult TimKiemNC()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XL_TimKiemNC(FormCollection fc)
        {

            string gia_vacine = fc["gia_vacine"].ToLower();
            string phongbenh = fc["phongbenh"].ToLower();
            string tenvaccine = fc["tenvaccine"].ToLower();
            List<VACCINE> lst = db.VACCINEs.ToList();
            if (!string.IsNullOrEmpty(gia_vacine))
            {
                if (gia_vacine == "duoi1")
                {
                    lst = lst.Where(l => l.gia_vacine < 1000000).ToList();
                }
                else if (gia_vacine == "khoang115")
                {
                    lst = lst.Where(l => l.gia_vacine >= 1000000 && l.gia_vacine < 1500000).ToList();
                }
                else if (gia_vacine == "khoang116")
                {
                    lst = lst.Where(l => l.gia_vacine >= 1500000 && l.gia_vacine < 2000000).ToList();
                }
                else if (gia_vacine == "khoang117")
                {
                    lst = lst.Where(l => l.gia_vacine >= 2000000 && l.gia_vacine < 2500000).ToList();
                }
                else if (gia_vacine == "khoang118")
                {
                    lst = lst.Where(l => l.gia_vacine >= 2500000 && l.gia_vacine < 3000000).ToList();
                }
                else if (gia_vacine == "khoang119")
                {
                    lst = lst.Where(l => l.gia_vacine >= 3000000 && l.gia_vacine < 3500000).ToList();
                }
                else if (gia_vacine == "tren35")
                {
                    lst = lst.Where(l => l.gia_vacine >= 3500000).ToList();
                }
            }
            if (!string.IsNullOrEmpty(phongbenh))
            {
                lst = lst.Where(l => l.phongbenh.ToLower().Contains(phongbenh)).ToList();
            }
            if (!string.IsNullOrEmpty(tenvaccine))
            {
                lst = lst.Where(l => l.ten_vaccine.ToLower().Contains(tenvaccine)).ToList();
            }
            if (lst.Count == 0)
            {
                ViewBag.Message = "Không tìm thấy Vaccine như Quý khách cần tìm";
            }
            return View("Index", lst);
            

        }


        //======================== Tra cuu Nguoi Dang Ky Tiem =========================
        public ActionResult IndexNguoiDangKyTiem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XL_TraCuuDangKyTiem(FormCollection fc)
        {
            string madangky = fc["madangky"].ToLower();
            List<NGUOITIEM_DANGKY> lst = new List<NGUOITIEM_DANGKY>();

            if (!string.IsNullOrEmpty(madangky))
            {
                lst = db.NGUOITIEM_DANGKies.Where(l => l.ma_dangky.ToLower().Contains(madangky)).ToList();
            }

            // Trả về view IndexNguoiDangKyTiem với danh sách NGUOITIEM_DANGKY
            return View("IndexNguoiDangKyTiem", lst);
        }

        //======================== Tra cuu Lich Su Tiem =========================
        public ActionResult IndexLichSuTiem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XL_TraCuuLichSuTiem(FormCollection fc)
        {
            string madangky = fc["madangky"].ToLower();
            List<TIEMVACCINE_MUI> lst = new List<TIEMVACCINE_MUI>();

            if (!string.IsNullOrEmpty(madangky))
            {
                lst = db.TIEMVACCINE_MUIs.Where(l => l.ma_dangky.ToLower().Contains(madangky)).ToList();
            }

            // Trả về view IndexNguoiDangKyTiem với danh sách NGUOITIEM_DANGKY
            return View("IndexLichSuTiem", lst);
        }
    }
}
