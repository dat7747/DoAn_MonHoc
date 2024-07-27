using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Web_TiemChungVaccine.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Web_TiemChungVaccine.Controllers
{
    public class KhachHangController : Controller
    {
        QL_VACCINEDataContext db = new QL_VACCINEDataContext();
        public ActionResult IndexKhachHang()
        {
            var vaccinesInStock = db.VACCINEs
                            .Join(db.KHOs,
                                  v => v.ma_vaccine,
                                  k => k.ma_vaccine,
                                  (v, k) => v)
                            .ToList();

            var maKhachHang = Session["maKH"]?.ToString();
            if (maKhachHang != null)
            {
                var updatedGioHangCount = db.GIOHANGs.Count(g => g.ma_khachhang == maKhachHang);
                Session["GioHangCount"] = updatedGioHangCount;
            }
            return View(vaccinesInStock);

        }
        //Khách Hàng xem chi tiết vaccine
        public ActionResult XemChiTietVaccine_KH(string id)
        {
            VACCINE s = db.VACCINEs.FirstOrDefault(i => i.ma_vaccine == id);
            Session["mavaccine"] = s.ma_vaccine;
            Session["tenvaccine"] = s.ten_vaccine;
            return View(s);
        }
        // Hiển thị Gói Combo 
        public ActionResult IndexCombo(/*int page = 1*/)
        {
            List<CHITIET_COMBO_VACCXINE> a = db.CHITIET_COMBO_VACCXINEs.ToList();
            return View(a);
        }
        //Khách Hàng xem chi tiết Combo vaccine
        public ActionResult XemChiTietCombo(string id)
        {
            CHITIET_COMBO_VACCXINE s = db.CHITIET_COMBO_VACCXINEs.FirstOrDefault(i => i.ma_combo == id);
            Session["macombo"] = s.ma_combo;
            Session["mavaccine"] = s.ma_vaccine;
            return View(s);
        }
        public ActionResult Voucher(/*int page = 1*/)
        {
            List<VOUCHER> a = db.VOUCHERs.ToList();
            return View(a);
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

        //Dang nhap
        public ActionResult DangNhapKH()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhapKH(FormCollection col)
        {
            KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(a => a.email_khachhang == col["email"] && a.pass_khachhang == col["pswd"]);
            if (kh != null)
            {
                if (kh.trangthai == 0) // Kiểm tra trạng thái tài khoản
                {
                    ModelState.AddModelError("myError", "Tài khoản chưa được xác thực gmail.");
                    return View(); // Trả về view với thông báo lỗi
                }

                // Lưu thông tin người dùng vào Session
                Session["kh"] = kh;
                Session["tenKH"] = kh.hoten_khachhang;
                Session["maKH"] = kh.ma_khachhang;
                Session["emailKH"] = kh.email_khachhang;
                Session["SDT"] = kh.sdt_khachhang;
                Session["ngaysinhKH"] = kh.ngaysinh_khachhang;
                Session["diemKH"] = kh.diemthanthiet;

                // Kiểm tra giỏ hàng và chuyển hướng đến trang Index của Giỏ Hàng
                var gioHang = db.GIOHANGs.Where(g => g.ma_khachhang == kh.ma_khachhang).ToList();
                var items = gioHang.Select(item => new {
                    Vaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == item.ma_vaccine),
                    SoLuong = item.soluong
                }).ToList();
                Session["GioHang"] = gioHang.Select(g => g.ma_vaccine).ToList();

                //return RedirectToAction("Index", "GioHang");
                return RedirectToAction("IndexKhachHang", "KhachHang");
            }
            else
            {
                ModelState.AddModelError("myError", "Invalid email and password.");
            }
            return View();
        }

        //==========================Đăng Xuất================================
        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa tất cả các biến Session
            return RedirectToAction("DangNhapKh", "KhachHang");
        }


        //==================== Đăng ký ==============================
        public ActionResult DangKyKH()
        {
            return View();
        }


        [HttpPost]
        public ActionResult DangKyKH(FormCollection f)
        {
            var hoten = f["dkytenkh"];
            var gioitinh = f["dkygioitinhkh"];
            DateTime ngaysinh;
            var email = f["dkyemailkh"];
            var sdt = f["dkysdtkh"];
            var matkhau = f["dkymatkhaukh"];
            var xacnhanmatkhau = f["dkyxacnhanmatkhaukh"];
            var tendn = ""; // Tạo mã khách hàng tự động dưới đây

            // Kiểm tra xem có trường nào bị bỏ trống không
            if (string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(gioitinh) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(matkhau) || string.IsNullOrEmpty(xacnhanmatkhau))
            {
                ModelState.AddModelError("myError", "Phải nhập đầy đủ thông tin.");
            }

            // Kiểm tra ngày sinh không được vượt quá ngày hiện tại
            if (!DateTime.TryParse(f["dkyngaysinhkh"], out ngaysinh) || ngaysinh > DateTime.Now)
            {
                ModelState.AddModelError("ngaysinhError", "Ngày sinh không được vượt quá ngày hiện tại.");
            }

            // Kiểm tra email đúng định dạng
            var emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                ModelState.AddModelError("emailError", "Email không đúng định dạng.");
            }

            //// Kiểm tra tên người dùng không có ký tự đặc biệt và chỉ chứa chữ cái có dấu tiếng Việt
            //var namePattern = @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂỄỆỉọỏốồổỗộớờởỡợụủứừửữựỳỵỷỹ\s]*$";
            //if (!Regex.IsMatch(hoten, namePattern))
            //{
            //    ModelState.AddModelError("nameError", "Tên người dùng không được chứa ký tự đặc biệt.");
            //}

            // Kiểm tra số điện thoại đủ 10 số và bắt đầu bằng số 0
            var phonePattern = @"^0\d{9}$";
            if (!Regex.IsMatch(sdt, phonePattern))
            {
                ModelState.AddModelError("sdtError", "Số điện thoại chưa đúng.");
            }

            // Kiểm tra mật khẩu trên 6 ký tự
            if (matkhau.Length < 6)
            {
                ModelState.AddModelError("passwordError", "Mật khẩu phải chứa ít nhất 6 ký tự.");
            }

            // Kiểm tra password trùng khớp
            if (matkhau != xacnhanmatkhau)
            {
                ModelState.AddModelError("confirmPasswordError", "Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa
            bool emailExists = db.KHACHHANGs.Any(k => k.email_khachhang == email);
            if (emailExists)
            {
                ModelState.AddModelError("emailExists", "Email đã tồn tại trong hệ thống.");
            }

            if (!ModelState.IsValid)
            {
                return View(); // Trả về view hiện tại với các lỗi ModelState
            }

            // Tạo mã khách hàng tự động
            string latestID = db.KHACHHANGs.OrderByDescending(k => k.ma_khachhang).Select(k => k.ma_khachhang).FirstOrDefault();
            string newID = "KH001"; // Giá trị mặc định
            if (!string.IsNullOrEmpty(latestID))
            {
                int number = int.Parse(latestID.Substring(2));
                number++;
                newID = "KH" + number.ToString("D3");
            }
            tendn = newID;

            // Tạo đối tượng KHACHHANG
            var khachHang = new KHACHHANG
            {
                ma_khachhang = tendn,
                hoten_khachhang = hoten,
                gioitinh_khachhang = gioitinh,
                ngaysinh_khachhang = ngaysinh,
                email_khachhang = email,
                sdt_khachhang = sdt,
                pass_khachhang = matkhau,
                trangthai = 0 // Đặt cờ xác nhận là 0 (false)
            };

            // Gửi email xác nhận
            try
            {
                MailMessage m = new MailMessage(
                    new MailAddress("nathadatiemchungvaccine2000@gmail.com", "He Thong Tiem Chung NATHADA"),
                    new MailAddress(email));
                m.Subject = "Email confirmation";
                m.Body = string.Format("Thân gửi {0},<br/>Cảm ơn bạn đã đăng ký tài khoản. " +
                    "Bạn vui lòng click vào link bên dưới để hoàn tất việc đăng ký:<br />" +
                    "<a href=\"{1}\" title=\"User Email Confirm\">{1}</a><br />" +
                    "Bạn sẽ được chuyển về trang ĐĂNG NHẬP, vui lòng nhập đúng tài khoản đã đăng ký để truy cập mua sắm.<br />Chúc một ngày tốt lành.",
                    hoten, Url.Action("ConfirmEmail", "KhachHang", new { Token = tendn, Email = email }, Request.Url.Scheme));
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("nathadatiemchungvaccine2000@gmail.com", "bdxs lrux wwte uqmp"),
                    EnableSsl = true
                };

                smtp.Send(m);

                // Thêm KHACHHANG vào cơ sở dữ liệu nếu gửi email thành công
                db.KHACHHANGs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email
                return RedirectToAction("Error", "Home"); // Điều hướng đến trang thông báo lỗi
            }

            return RedirectToAction("XacThuc", "KhachHang", new { Email = email });
        }


        public ActionResult XacThuc(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public ActionResult ConfirmEmail(string Token, string Email)
        {
            var user = db.KHACHHANGs.SingleOrDefault(k => k.ma_khachhang == Token && k.email_khachhang == Email);
            if (user != null)
            {
                user.trangthai = 1; // Cập nhật cờ xác nhận là 1 (true)
                db.SubmitChanges();
                Session["ok"] = "Tài khoản " + Token + " đã được đăng ký thành công!";
                ViewBag.success = "OK";
                return RedirectToAction("DangNhapKH", "KhachHang");
            }
            else
            {
                return RedirectToAction("XacThuc", "KhachHang", new { Email = "" });
            }
        }

        //======================== Load trang cá nhân =========================
        public ActionResult TrangCaNhan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TrangCaNhan(FormCollection fc)
        {

            string makh = fc["ma"].ToLower();
            List<KHACHHANG> lst = db.KHACHHANGs.ToList();
            if (!string.IsNullOrEmpty(makh))
            {
                lst = lst.Where(l => l.ma_khachhang.ToLower().Contains(makh)).ToList();
            }
            return View("TrangCaNhan", lst);
        }
        //======================== Load trang cá nhân =========================
        public ActionResult TrangLichSuTiemChung()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TrangLichSuTiemChung(FormCollection fc)
        {

            string makhtc = fc["ma"].ToLower();
            List<NGUOITIEM_DANGKY> lst = db.NGUOITIEM_DANGKies.ToList();
            if (!string.IsNullOrEmpty(makhtc))
            {
                lst = lst.Where(l => l.ma_khachhang.ToLower().Contains(makhtc)).ToList();
            }
            return View("TrangLichSuTiemChung", lst);
        }
        //====================== Sửa thông tin khách hàng=======================
        //public ActionResult Sua_ThongTinCaNhan_KhachHang(String id)
        //{
        //    KHACHHANG kh = db.KHACHHANGs.Where(row => row.ma_khachhang == id).FirstOrDefault();
        //    return View(kh);
        //}
        //[HttpPost]
        //public ActionResult Sua_ThongTinCaNhan_KhachHang(KHACHHANG kaha)
        //{
        //    KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(row => row.ma_khachhang == kaha.ma_khachhang);
        //    if (kh == null)
        //    {
        //        ModelState.AddModelError("Error", "Không có dữ liệu");
        //        return View();
        //    }
        //    else
        //    {
        //        kh.hoten_khachhang = kaha.hoten_khachhang;
        //        kh.sdt_khachhang = kaha.sdt_khachhang;
        //        kh.ngaysinh_khachhang = kaha.ngaysinh_khachhang;
        //        kh.gioitinh_khachhang = kaha.gioitinh_khachhang;
        //        db.SubmitChanges();
        //        return View("KhachHang", "IndexKhachHang");
        //    }
        //}
        //Sửa Thông tin cá nhân
        public ActionResult Sua_ThongTinCaNhan_KH(String id)
        {
            KHACHHANG kh = db.KHACHHANGs.Where(row => row.ma_khachhang == id).FirstOrDefault();
            return View(kh);
        }

        //Sua thong tin Khach Hang
        [HttpPost]
        public async Task<ActionResult> Sua_ThongTinCaNhan_KH(KHACHHANG kaha)
        {
            KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(row => row.ma_khachhang == kaha.ma_khachhang);
            if (kh == null)
            {
                ModelState.AddModelError("Error", "Không có dữ liệu");
                return View();
            }
            else
            {
                kh.hoten_khachhang = kaha.hoten_khachhang;
                kh.sdt_khachhang = kaha.sdt_khachhang;
                kh.ngaysinh_khachhang = kaha.ngaysinh_khachhang;
                kh.gioitinh_khachhang = kaha.gioitinh_khachhang;
                db.SubmitChanges();

                // Dừng lại 3 giây
                await Task.Delay(3000);

                return RedirectToAction("IndexKhachHang");
            }
        }

        //=================== Đổi mật khẩu =====================================
        public ActionResult Doi_MatKhau_KH(String id)
        {
            KHACHHANG kh = db.KHACHHANGs.Where(row => row.ma_khachhang == id).FirstOrDefault();
            return View(kh);
        }
        [HttpPost]
        public async Task<ActionResult> Doi_MatKhau_KH(KHACHHANG kaha)
        {
            KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(row => row.ma_khachhang == kaha.ma_khachhang);
            if (kh == null)
            {
                ModelState.AddModelError("Error", "Không có dữ liệu");
                return View();
            }
            else
            {
                // Kiểm tra độ dài mật khẩu
                if (kaha.pass_khachhang.Length < 6)
                {
                    ModelState.AddModelError("pass_khachhang", "Mật khẩu phải đủ 6 kí tự trở lên");
                    return View(kaha);
                }

                kh.pass_khachhang = kaha.pass_khachhang;
                db.SubmitChanges();
                List<VACCINE> u = db.VACCINEs.ToList();
                //return TrangCaNhan();
                // Dừng lại 3 giây
                await Task.Delay(3000);
                return RedirectToAction("IndexKhachHang");
            }
        }


        //Lich su tiem Vaccine
        [HttpPost]
        public ActionResult LichSuTiemVC(string ma)
        {
            var nguoiTiemDangKy = db.NGUOITIEM_DANGKies.Where(b => b.ma_khachhang == ma).ToList();
            var lichSuTiem = new List<TIEMVACCINE_MUI>();

            foreach (var item in nguoiTiemDangKy)
            {
                var muiTiem = db.TIEMVACCINE_MUIs.Where(t => t.ma_dangky == item.ma_dangky).ToList();
                lichSuTiem.AddRange(muiTiem);
            }

            return View(lichSuTiem);
        }

        //Thong Tin Dang Ky Tiem Chung
        [HttpPost]
        public ActionResult ThongTinDangKyTiemChung(string ma)
        {
            List<NGUOITIEM_DANGKY> nguoiDKT = db.NGUOITIEM_DANGKies.Where(b => b.ma_khachhang == ma).ToList();
            return View(nguoiDKT);
        }


        //Khách Hàng xem chi tiết Lich su tiem
        public ActionResult XemChiTietLichSuTiem_KH(string id)
        {
            NGUOITIEM_DANGKY s = db.NGUOITIEM_DANGKies.FirstOrDefault(i => i.ma_dangky == id);
            Session["madangky"] = s.ma_dangky;
            Session["makhachhang"] = s.ma_khachhang;
            return View(s);
        }

        public ActionResult Sua_ThongTinLichTiem_KH(String id)
        {
            NGUOITIEM_DANGKY ntdk = db.NGUOITIEM_DANGKies.Where(row => row.ma_dangky == id).FirstOrDefault();
            return View(ntdk);
        }
        [HttpPost]
        public async Task<ActionResult> Sua_ThongTinLichTiem_KH(NGUOITIEM_DANGKY nguoitiemdangky)
        {
            NGUOITIEM_DANGKY ntdk = db.NGUOITIEM_DANGKies.FirstOrDefault(row => row.ma_dangky == nguoitiemdangky.ma_dangky);
            if (ntdk == null)
            {
                ModelState.AddModelError("Error", "Không có dữ liệu");
                return View();
            }
            else
            {
                ntdk.hoten_nguoitiem = nguoitiemdangky.hoten_nguoitiem;
                ntdk.ngaysinh_nguoitiem = nguoitiemdangky.ngaysinh_nguoitiem;
                ntdk.gioitinh_nguoitiem = nguoitiemdangky.gioitinh_nguoitiem;
                ntdk.diachi_nguoitiem = nguoitiemdangky.diachi_nguoitiem;
                ntdk.hoten_nguoilienhe = nguoitiemdangky.hoten_nguoilienhe;
                ntdk.moiquanhe_nguoitiem = nguoitiemdangky.moiquanhe_nguoitiem;
                ntdk.sdt_nguoilienhe = nguoitiemdangky.sdt_nguoilienhe;
                ntdk.ngay_muontiem = nguoitiemdangky.ngay_muontiem;
                db.SubmitChanges();

                // Dừng lại 3 giây
                await Task.Delay(3000);

                return RedirectToAction("IndexKhachHang");
            }
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
            return View("IndexKhachHang", lst);


        }
       
    }
}
